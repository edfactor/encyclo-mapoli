using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Caching;

/// <summary>
/// Cached lookup for profit codes. ProfitCodes table is small (~20 records) and never changes.
/// Cache eliminates repeated database queries during profit calculations.
/// </summary>
public sealed class ProfitCodeCache : LookupCache<byte, ProfitCode, ProfitCode>
{
    public ProfitCodeCache(
        IDistributedCache cache,
        IProfitSharingDataContextFactory contextFactory,
        ILogger<ProfitCodeCache> logger)
        : base(
            cache,
            contextFactory,
            logger,
            lookupName: "ProfitCodes",
            queryBuilder: query => query, // No filtering needed
            keySelector: entity => entity.Id,
            valueSelector: entity => entity, // Return full entity
            getDbSetFunc: ctx => ctx.ProfitCodes, // Get DbSet from read-only context
            absoluteExpiration: TimeSpan.FromHours(24)) // Profit codes are effectively static
    {
    }
}
