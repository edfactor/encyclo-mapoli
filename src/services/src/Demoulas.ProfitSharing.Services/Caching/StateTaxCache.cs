using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Caching;

/// <summary>
/// Cached lookup for state tax rates. StateTaxes table is small (~50 records) and rarely changes.
/// Cache significantly reduces database load during year-end processing where this is accessed 1000+ times.
/// </summary>
public sealed class StateTaxCache : LookupCache<string, decimal?, StateTax>
{
    public StateTaxCache(
        IDistributedCache cache,
        IProfitSharingDataContextFactory contextFactory,
        ILogger<StateTaxCache> logger)
        : base(
            cache,
            contextFactory,
            logger,
            lookupName: "StateTaxes",
            queryBuilder: query => query, // No filtering needed
            keySelector: entity => entity.Abbreviation,
            valueSelector: entity => entity.Rate, // Nullable to distinguish "exists with 0" from "not found"
            getDbSetFunc: ctx => ctx.StateTaxes, // Get DbSet from read-only context
            absoluteExpiration: TimeSpan.FromHours(4)) // State taxes rarely change
    {
    }
}
