using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Metrics;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Handles matching logic for demographics ingestion.
/// Provides primary matching by OracleHcmId and fallback matching by (SSN, BadgeNumber) pairs.
/// </summary>
public sealed class DemographicMatchingService : IDemographicMatchingService
{
    private readonly IDemographicsRepository _repository;
    private readonly ILogger<DemographicMatchingService> _logger;

    public DemographicMatchingService(
        IDemographicsRepository repository,
        ILogger<DemographicMatchingService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<Demographic>> MatchByOracleIdAsync(
        Dictionary<long, Demographic> incomingByOracleId,
        CancellationToken ct)
    {
        var oracleIds = incomingByOracleId.Keys.ToList();

        var matched = await _repository.GetByOracleIdsAsync(oracleIds, ct).ConfigureAwait(false);

        if (matched.Count > 0)
        {
            DemographicsIngestMetrics.PrimaryMatched.Add(matched.Count);
            _logger.LogDebug("Primary match by OracleHcmId found {Count} demographics", matched.Count);
        }

        return matched;
    }

    public async Task<(List<Demographic> Matched, bool SkippedAllZeroBadge)> MatchByFallbackAsync(
        List<(int Ssn, int BadgeNumber)> fallbackPairs,
        CancellationToken ct)
    {
        if (fallbackPairs.Count == 0)
        {
            return (new List<Demographic>(), false);
        }

        DemographicsIngestMetrics.FallbackPairs.Add(fallbackPairs.Count);

        // Check if all badges are zero (degenerate case)
        if (fallbackPairs.All(p => p.BadgeNumber == 0))
        {
            _logger.LogCritical(
                "All fallback demographic pairs have BadgeNumber == 0. " +
                "Aborting fallback lookup. FallbackPairs={FallbackPairs}",
                fallbackPairs.Count);

            DemographicsIngestMetrics.FallbackSkippedAllZeroBadge.Add(1);
            return (new List<Demographic>(), true);
        }

        var matched = await _repository.GetBySsnAndBadgePairsAsync(fallbackPairs, ct).ConfigureAwait(false);

        if (matched.Count > 0)
        {
            DemographicsIngestMetrics.FallbackMatched.Add(matched.Count);
            _logger.LogDebug("Fallback match by (SSN, Badge) found {Count} demographics", matched.Count);
        }

        return (matched, false);
    }

    public List<Demographic> IdentifyNewDemographics(
        List<Demographic> incoming,
        List<Demographic> existing)
    {
        var existingOracleIds = existing.Select(e => e.OracleHcmId).ToHashSet();
        var existingSsnBadges = existing.Select(e => (e.Ssn, e.BadgeNumber)).ToHashSet();

        var newDemographics = incoming
            .Where(e => !existingOracleIds.Contains(e.OracleHcmId)
                     && !existingSsnBadges.Contains((e.Ssn, e.BadgeNumber)))
            .ToList();

        if (newDemographics.Count > 0)
        {
            _logger.LogDebug("Identified {Count} new demographics for insertion", newDemographics.Count);
        }

        return newDemographics;
    }
}
