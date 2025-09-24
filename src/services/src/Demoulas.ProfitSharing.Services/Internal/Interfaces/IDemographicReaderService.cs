using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Services.Internal.Interfaces;

/// <summary>
/// Chooses live or frozen data **per request** and records the choice in <see cref="HttpContext.Items"/>.
/// </summary>
public interface IDemographicReaderService
{
    Task<IQueryable<Demographic>> BuildDemographicQuery(IProfitSharingDbContext ctx, bool useFrozenData = false);
    /// <summary>
    /// Builds a demographic query representing data as-of a specific point in time (ignores active frozen state).
    /// </summary>
    Task<IQueryable<Demographic>> BuildDemographicQueryAsOf(IProfitSharingDbContext ctx, DateTimeOffset asOf);
}
