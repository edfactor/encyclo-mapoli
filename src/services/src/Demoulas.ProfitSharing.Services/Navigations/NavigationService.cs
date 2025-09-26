using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Navigations;

public class NavigationService : INavigationService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IAppUser _appUser;
    private readonly ILogger<NavigationService>? _logger;

    public NavigationService(IProfitSharingDataContextFactory dataContextFactory, IAppUser appUser, ILogger<NavigationService>? logger = null)
    {
        _dataContextFactory = dataContextFactory;
        _appUser = appUser;
        _logger = logger;
    }


    public async Task<List<NavigationDto>> GetNavigation(CancellationToken cancellationToken)
    {
        var roleNamesUpper = _appUser.GetUserAllRoles()
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r!.ToUpper())
            .ToList();

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
        });

        // Check if the current user has any read-only roles based on database configuration
        var userRoles = _appUser.GetUserAllRoles()?.Where(r => !string.IsNullOrWhiteSpace(r)).ToList() ?? new List<string>();
        var userHasReadOnlyRole = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var readOnlyRoleNames = await context.NavigationRoles
                .Where(nr => nr.IsReadOnly)
                .Select(nr => nr.Name.ToUpper())
                .ToListAsync(cancellationToken);

            return userRoles.Any(userRole => 
                readOnlyRoleNames.Contains(userRole.ToUpper()));
        });

        var lookup = flatList.ToLookup(x => x.ParentId);
        // helper to get raw required roles (uppercase) from entity
        static List<string> EntityRolesToUpper(System.Collections.Generic.ICollection<Data.Entities.Navigations.NavigationRole>? roles)
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
                        var intersection = entityRolesUpper.Intersect(parentEffectiveRoles).ToList();
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
                            IsNavigable = p.IsNavigable.HasValue ? p.IsNavigable.Value : !string.IsNullOrWhiteSpace(p.Url),
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
                        IsNavigable = x.IsNavigable.HasValue ? x.IsNavigable.Value : !string.IsNullOrWhiteSpace(x.Url),
                        IsReadOnly = userHasReadOnlyRole
                    };
                })
                .ToList();
        }

        return BuildTree(null); // root level
    }


    public NavigationDto GetNavigation(short navigationId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<NavigationStatusDto>> GetNavigationStatus(CancellationToken cancellationToken)
    {
        var navigationStatusList = await _dataContextFactory.UseReadOnlyContext(context =>
            context.NavigationStatuses.Select(x => new NavigationStatusDto { Id = x.Id, Name = x.Name }).ToListAsync(cancellationToken)
        );
        return navigationStatusList;
    }


    public async Task<bool> UpdateNavigation(short navigationId, byte statusId, CancellationToken cancellationToken)
    {
        var success = await _dataContextFactory.UseWritableContext(async context =>
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
        return success > 0;
    }
}
