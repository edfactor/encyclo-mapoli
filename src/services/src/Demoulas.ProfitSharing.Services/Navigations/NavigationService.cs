using System.Text.Json;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Navigations;

public class NavigationService : INavigationService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IAppUser _appUser;
    private readonly ILogger<NavigationService>? _logger;
    private readonly IDistributedCache _distributedCache;
    private const string NavigationStatusCacheKey = "navigation-status-all";
    private const string NavigationCacheKeyPrefix = "navigation-tree-";

    public NavigationService(IProfitSharingDataContextFactory dataContextFactory, IAppUser appUser, IDistributedCache distributedCache, ILogger<NavigationService>? logger = null)
    {
        _dataContextFactory = dataContextFactory;
        _appUser = appUser;
        _distributedCache = distributedCache;
        _logger = logger;
    }


    public async Task<List<NavigationDto>> GetNavigation(CancellationToken cancellationToken)
    {
        List<string> roleNamesUpper = _appUser.GetUserAllRoles()
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r!.Trim().ToUpper()) // Trim whitespace to ensure consistency
            .Distinct() // Remove any duplicate roles
            .OrderBy(r => r) // Sort for consistent cache key
            .ToList();

        // Get current cache version to ensure cache is busted when navigation status is updated
        const string versionKey = "navigation-tree-version";
        byte[]? versionBytes = await _distributedCache.GetAsync(versionKey, cancellationToken);
        int version = versionBytes is { Length: > 0 }
            ? BitConverter.ToInt32(versionBytes, 0)
            : 0;

        // Create cache key based on sorted role names and version for consistent caching
        // Use pipe separator to clearly delimit roles and avoid ambiguity
        string roleKey = string.Join("|", roleNamesUpper);
        string cacheKey = $"{NavigationCacheKeyPrefix}v{version}-{roleKey}";

        _logger?.LogDebug("Navigation cache key generated: {CacheKey} (roles: {Roles})", cacheKey, roleKey);

        // Try to get from distributed cache first
        byte[]? cachedBytes = await _distributedCache.GetAsync(cacheKey, cancellationToken);
        if (cachedBytes != null)
        {
            var cachedNavigation = JsonSerializer.Deserialize<List<NavigationDto>>(cachedBytes);
            if (cachedNavigation != null)
            {
                _logger?.LogDebug("Navigation tree loaded from distributed cache (version {Version}) for roles: {Roles}", version, roleKey);
                return cachedNavigation;
            }
        }

        // Cache miss - load from database
        var flatList = await _dataContextFactory.UseReadOnlyContext(context =>
        {
            var query = context.Navigations
                .Where(n => n.RequiredRoles!.Any(rr => roleNamesUpper.Contains(rr.Name.ToUpper())))
                .OrderBy(n => n.OrderNumber)
                .Include(n => n.RequiredRoles)
                .Include(n => n.NavigationStatus)
                .Include(n => n.PrerequisiteNavigations!) // prerequisites
                    .ThenInclude(p => p.NavigationStatus)
                .Include(n => n.PrerequisiteNavigations!)
                    .ThenInclude(p => p.RequiredRoles);

            return query.ToListAsync(cancellationToken);
        }, cancellationToken);

        // Check if the current user has any read-only roles based on database configuration
        var userRoles = _appUser.GetUserAllRoles()?.Where(r => !string.IsNullOrWhiteSpace(r)).ToList() ?? new List<string>();
        bool userHasReadOnlyRole = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var readOnlyRoleNames = await context.NavigationRoles
                .Where(nr => nr.IsReadOnly)
                .Select(nr => nr.Name.ToUpper())
                .ToListAsync(cancellationToken);

            return userRoles.Any(userRole =>
                readOnlyRoleNames.Contains(userRole.ToUpper()));
        }, cancellationToken);

        var lookup = flatList.ToLookup(x => x.ParentId);
        // helper to get raw required roles (uppercase) from entity
        static List<string> EntityRolesToUpper(ICollection<Data.Entities.Navigations.NavigationRole>? roles)
        {
            return roles == null
                ? new List<string>()
                : roles.Where(r => !string.IsNullOrWhiteSpace(r.Name)).Select(r => r.Name.ToUpper()).ToList();
        }
        // Build tree while enforcing permission inheritance and that child roles are never broader than parent.
        List<NavigationDto> BuildTree(short? parentId, List<string>? parentEffectiveRoles = null)
        {
            parentEffectiveRoles ??= new List<string>();

            return lookup[parentId]
                .Select(x =>
                {
                    // compute entity roles (upper-case)
                    var entityRolesUpper = EntityRolesToUpper(x.RequiredRoles);

                    // determine effective roles: if entity has none, inherit from parent; otherwise intersect with parent (if parent has roles)
                    List<string> effectiveRolesUpper;
                    if (entityRolesUpper == null || entityRolesUpper.Count == 0)
                    {
                        effectiveRolesUpper = new List<string>(parentEffectiveRoles);
                    }
                    else if (parentEffectiveRoles == null || parentEffectiveRoles.Count == 0)
                    {
                        // top-level or parent has no roles -> child's specified roles are allowed
                        effectiveRolesUpper = entityRolesUpper;
                    }
                    else
                    {
                        // intersect child with parent so child cannot have broader permissions
                        List<string> intersection = entityRolesUpper.Intersect(parentEffectiveRoles).ToList();
                        if (intersection.Count != entityRolesUpper.Count)
                        {
                            // log a warning that child's specified roles were broader than parent; note intersection applied
                            try
                            {
                                _logger?.LogWarning("Navigation id {NavigationId} has RequiredRoles that are broader than its parent; applying intersection of specified roles and parent roles.", x.Id);
                            }
                            catch
                            {
                                // ignore logger failures
                            }
                        }
                        effectiveRolesUpper = intersection;
                    }

                    // Map required role names back to original casing from entity but only include those that are in effectiveRolesUpper and in current user's roles
                    var requiredRolesForDto = x.RequiredRoles == null
                        ? (effectiveRolesUpper?.ToList() ?? new List<string>())
                        : x.RequiredRoles.Where(r => !string.IsNullOrWhiteSpace(r.Name) && effectiveRolesUpper.Contains(r.Name.ToUpper())).Select(r => r.Name).ToList();

                    // Items (children) inherit effectiveRolesUpper as their parent roles
                    var items = BuildTree(x.Id, effectiveRolesUpper);

                    // Map prerequisite navigations (keep simple mapping, but enforce that their RequiredRoles are filtered by current user's roles)
                    var prereqs = x.PrerequisiteNavigations?
                        .Select(p => new NavigationDto
                        {
                            Id = p.Id,
                            ParentId = p.ParentId,
                            Title = p.Title,
                            SubTitle = p.SubTitle,
                            Url = p.Url,
                            StatusId = p.StatusId,
                            StatusName = p.NavigationStatus!.Name,
                            OrderNumber = p.OrderNumber,
                            Icon = p.Icon,
                            RequiredRoles = p.RequiredRoles?.Where(r => roleNamesUpper.Contains(r.Name.ToUpper())).Select(m => m.Name).ToList(),
                            Disabled = p.Disabled,
                            Items = null,
                            PrerequisiteNavigations = new List<NavigationDto>(),
                            // Prefer the DB-backed flag when present; otherwise fall back to Url heuristic.
                            IsNavigable = p.IsNavigable ?? !string.IsNullOrWhiteSpace(p.Url),
                            IsReadOnly = userHasReadOnlyRole
                        })
                        .ToList() ?? new List<NavigationDto>();

                    return new NavigationDto
                    {
                        Id = x.Id,
                        Icon = x.Icon,
                        OrderNumber = x.OrderNumber,
                        ParentId = x.ParentId,
                        StatusId = x.StatusId,
                        StatusName = x.NavigationStatus?.Name,
                        Title = x.Title,
                        Url = x.Url,
                        SubTitle = x.SubTitle,
                        Items = items,
                        Disabled = x.Disabled,
                        RequiredRoles = (requiredRolesForDto?.Where(r => roleNamesUpper.Contains(r.ToUpper())).ToList()) ?? new List<string>(),
                        // Project prerequisite navigations that are currently completed.
                        PrerequisiteNavigations = prereqs,
                        // Prefer the DB-backed IsNavigable when present; otherwise fall back to Url-derived logic
                        // Prefer the DB-backed flag when present; otherwise fall back to Url heuristic.
                        IsNavigable = x.IsNavigable ?? !string.IsNullOrWhiteSpace(x.Url),
                        IsReadOnly = userHasReadOnlyRole
                    };
                })
                .ToList();
        }

        var navigationTree = BuildTree(null); // root level

        // Store in distributed cache for 15 minutes (half of previous 30 min) with 7.5 min sliding
        // This ensures navigation changes propagate faster to users
        byte[] serialized = JsonSerializer.SerializeToUtf8Bytes(navigationTree);
        DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            SlidingExpiration = TimeSpan.FromMinutes(7.5) // Reduced from 15
        };
        await _distributedCache.SetAsync(cacheKey, serialized, cacheOptions, cancellationToken);

        _logger?.LogInformation("Navigation tree loaded from database and cached (version {Version}) for roles: {Roles} ({Count} root items)", version, roleKey, navigationTree.Count);

        return navigationTree;
    }


    public async Task<List<NavigationStatusDto>> GetNavigationStatus(CancellationToken cancellationToken)
    {
        // Try to get from distributed cache first
        byte[]? cachedBytes = await _distributedCache.GetAsync(NavigationStatusCacheKey, cancellationToken);
        if (cachedBytes != null)
        {
            var cachedList = JsonSerializer.Deserialize<List<NavigationStatusDto>>(cachedBytes);
            if (cachedList != null)
            {
                _logger?.LogDebug("Navigation status loaded from distributed cache");
                return cachedList;
            }
        }

        // Cache miss - load from database
        var navigationStatusList = await _dataContextFactory.UseReadOnlyContext(context =>
            context.NavigationStatuses.Select(x => new NavigationStatusDto { Id = x.Id, Name = x.Name }).ToListAsync(cancellationToken)
, cancellationToken);

        // Store in distributed cache for 15 minutes (half of previous 30 min) with 7.5 min sliding
        // This ensures navigation status changes propagate faster
        byte[] serialized = JsonSerializer.SerializeToUtf8Bytes(navigationStatusList);
        DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15), // Reduced from 30
            SlidingExpiration = TimeSpan.FromMinutes(7.5) // Reduced from 15
        };
        await _distributedCache.SetAsync(NavigationStatusCacheKey, serialized, cacheOptions, cancellationToken);

        _logger?.LogInformation("Navigation status loaded from database and cached ({Count} statuses)", navigationStatusList.Count);
        return navigationStatusList;
    }


    public async Task<bool> UpdateNavigation(short navigationId, byte statusId, CancellationToken cancellationToken)
    {
        int success = await _dataContextFactory.UseWritableContext(async context =>
        {
            //update navigation status
            var nav = await context.Navigations.FirstOrDefaultAsync(x => x.Id == navigationId, cancellationToken);
            if (nav == null)
            {
                return 0;
            }

            nav.StatusId = statusId;

            //Navigation Tracker
            await context.NavigationTrackings.AddAsync(
                new Data.Entities.Navigations.NavigationTracking()
                {
                    NavigationId = navigationId,
                    StatusId = statusId,
                    Username = _appUser.UserName ?? "",
                    LastModified = DateTimeOffset.UtcNow
                }, cancellationToken);
            return await context.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        // Invalidate both navigation status cache and all navigation tree caches after successful update
        if (success > 0)
        {
            await _distributedCache.RemoveAsync(NavigationStatusCacheKey, cancellationToken);

            // Note: We cannot efficiently remove all navigation tree caches (since they're keyed by role combinations)
            // without pattern-based cache invalidation. Options:
            // 1. Use cache tags (if Redis supports it)
            // 2. Track all active role combinations (complex)
            // 3. Use a cache version key (increment on update, check on read)
            // 4. Accept short stale data (cache expires in 30 min anyway)
            // 
            // For now, we'll implement a simple version key approach.
            // When UpdateNavigation is called, we increment a version counter stored in cache.
            // GetNavigation includes this version in the cache key, so any version bump invalidates all cached navigation trees.

            await BustAllNavigationTreeCaches(cancellationToken);

            _logger?.LogInformation("Navigation caches invalidated after navigation {NavigationId} update to status {StatusId}", navigationId, statusId);
        }

        return success > 0;
    }

    /// <summary>
    /// Busts all navigation tree caches by incrementing a version counter.
    /// This effectively invalidates all role-based navigation tree cache entries.
    /// </summary>
    private async Task BustAllNavigationTreeCaches(CancellationToken cancellationToken)
    {
        const string versionKey = "navigation-tree-version";

        try
        {
            // Get current version (default to 0 if not exists)
            byte[]? currentVersionBytes = await _distributedCache.GetAsync(versionKey, cancellationToken);
            int currentVersion = currentVersionBytes != null && currentVersionBytes.Length > 0
                ? BitConverter.ToInt32(currentVersionBytes, 0)
                : 0;

            // Increment version
            int newVersion = currentVersion + 1;
            byte[] newVersionBytes = BitConverter.GetBytes(newVersion);

            // Store new version (never expires - small 4-byte value)
            await _distributedCache.SetAsync(versionKey, newVersionBytes, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = null // No expiration
            }, cancellationToken);

            _logger?.LogDebug("Navigation tree cache version incremented from {OldVersion} to {NewVersion}", currentVersion, newVersion);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to bust navigation tree caches");
        }
    }

    /// <summary>
    /// Resets all navigation statuses to 'Not Started' and invalidates all navigation tree caches.
    /// This is typically called when a new demographics freeze point is created to restart workflows.
    /// </summary>
    public async Task ResetAllStatusesToNotStartedAsync(CancellationToken cancellationToken = default)
    {
        await _dataContextFactory.UseWritableContext(async (context) =>
        {
            // Reset all navigation statuses to 'Not Started' using ExecuteUpdateAsync for efficiency
            // Filter: Only update statuses that are currently set and not already 'Not Started'
            int rowsUpdated = await context.Navigations
                .Where(n => n.StatusId != null && n.StatusId != Data.Entities.Navigations.NavigationStatus.Constants.NotStarted)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(n => n.StatusId, Data.Entities.Navigations.NavigationStatus.Constants.NotStarted),
                    cancellationToken);

            _logger?.LogInformation("Reset {RowCount} navigation statuses to 'Not Started'", rowsUpdated);
            return rowsUpdated;
        }, cancellationToken);

        // Invalidate all navigation tree caches so status changes are immediately visible
        await BustAllNavigationTreeCaches(cancellationToken);

        _logger?.LogInformation("All navigation statuses reset to 'Not Started' and caches invalidated");
    }
}

