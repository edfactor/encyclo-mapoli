using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Demoulas.ProfitSharing.Services;

/// <summary>
/// Service for retrieving vesting schedule information with distributed caching.
/// </summary>
/// <remarks>
/// Uses version-based cache invalidation pattern. Cache keys include version number
/// to allow instant invalidation when vesting schedules are updated.
/// Cache entries do not expire (infinite TTL) - invalidation is done by incrementing version.
/// </remarks>
public sealed class VestingScheduleService : IVestingScheduleService
{
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly IDistributedCache _cache;

    private const string CacheKeyPrefix = "VestingSchedule";
    private const string VersionKey = "VestingSchedule:Version";

    public VestingScheduleService(
        IProfitSharingDataContextFactory contextFactory,
        IDistributedCache cache)
    {
        _contextFactory = contextFactory;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<decimal> GetVestingPercentAsync(int scheduleId, int yearsOfService, CancellationToken ct = default)
    {
        var version = await GetCacheVersionAsync(ct);
        var cacheKey = $"{CacheKeyPrefix}:Detail:{scheduleId}:v{version}";

        // Try to get from cache
        var cachedJson = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cachedJson))
        {
            var cachedDetails = JsonSerializer.Deserialize<List<VestingScheduleDetailCache>>(cachedJson);
            if (cachedDetails != null)
            {
                return GetPercentForYears(cachedDetails, yearsOfService);
            }
        }

        // Not in cache, fetch from database
        var details = await _contextFactory.UseReadOnlyContext(async ctx =>
        {
            return await ctx.VestingScheduleDetails
                .Where(d => d.VestingScheduleId == scheduleId)
                .OrderBy(d => d.YearsOfService)
                .Select(d => new VestingScheduleDetailCache
                {
                    YearsOfService = d.YearsOfService,
                    VestingPercent = d.VestingPercent
                })
                .ToListAsync(ct);
        }, ct);

        // Cache the result (no expiration - version-based invalidation)
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = null // No expiration
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(details), options, ct);

        return GetPercentForYears(details, yearsOfService);
    }

    /// <inheritdoc />
    public async Task<int> GetNewPlanEffectiveYearAsync(CancellationToken ct = default)
    {
        var version = await GetCacheVersionAsync(ct);
        var cacheKey = $"{CacheKeyPrefix}:NewPlanYear:v{version}";

        // Try to get from cache
        var cachedJson = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cachedJson))
        {
            return JsonSerializer.Deserialize<int>(cachedJson);
        }

        // Not in cache, fetch from database
        var effectiveYear = await _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var schedule = await ctx.VestingSchedules
                .Where(s => s.Id == VestingSchedule.Constants.NewPlan)
                .Select(s => s.EffectiveDate.Year)
                .FirstOrDefaultAsync(ct);

            return schedule;
        }, ct);

        // Cache the result (no expiration - version-based invalidation)
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = null // No expiration
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(effectiveYear), options, ct);

        return effectiveYear;
    }

    /// <summary>
    /// Gets the vesting percentage for a given number of years of service.
    /// Uses the highest years-of-service entry that is less than or equal to the given years.
    /// </summary>
    private static decimal GetPercentForYears(List<VestingScheduleDetailCache> details, int yearsOfService)
    {
        // Find the highest years entry that is <= yearsOfService
        var matchingDetail = details
            .Where(d => d.YearsOfService <= yearsOfService)
            .OrderByDescending(d => d.YearsOfService)
            .FirstOrDefault();

        return matchingDetail?.VestingPercent ?? 0;
    }

    /// <summary>
    /// Gets the current cache version number.
    /// Version is stored in cache and never expires.
    /// </summary>
    private async Task<int> GetCacheVersionAsync(CancellationToken ct)
    {
        var versionJson = await _cache.GetStringAsync(VersionKey, ct);
        if (!string.IsNullOrEmpty(versionJson) && int.TryParse(versionJson, out var version))
        {
            return version;
        }

        // Initialize version to 1 if not found
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = null // No expiration
        };
        await _cache.SetStringAsync(VersionKey, "1", options, ct);
        return 1;
    }

    /// <summary>
    /// Internal cache model for vesting schedule details.
    /// </summary>
    private sealed class VestingScheduleDetailCache
    {
        public int YearsOfService { get; set; }
        public decimal VestingPercent { get; set; }
    }
}
