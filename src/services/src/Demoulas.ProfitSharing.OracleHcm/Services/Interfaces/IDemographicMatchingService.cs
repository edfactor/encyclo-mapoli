using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;

/// <summary>
/// Service for matching incoming demographics against existing records.
/// Handles primary matching by OracleHcmId and fallback matching by (SSN, BadgeNumber) pairs.
/// </summary>
public interface IDemographicMatchingService
{
    /// <summary>
    /// Matches incoming demographics using primary OracleHcmId lookup.
    /// </summary>
    /// <param name="incomingByOracleId">Dictionary of incoming demographics keyed by OracleHcmId</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of existing demographics that matched by OracleHcmId</returns>
    Task<List<Demographic>> MatchByOracleIdAsync(
        Dictionary<long, Demographic> incomingByOracleId,
        CancellationToken ct);

    /// <summary>
    /// Matches demographics using fallback (SSN, BadgeNumber) pairs.
    /// Filters out zero badge numbers to prevent degenerate queries.
    /// </summary>
    /// <param name="fallbackPairs">List of (SSN, BadgeNumber) pairs to match</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Tuple of (matched demographics, whether all badges were zero)</returns>
    Task<(List<Demographic> Matched, bool SkippedAllZeroBadge)> MatchByFallbackAsync(
        List<(int Ssn, int BadgeNumber)> fallbackPairs,
        CancellationToken ct);

    /// <summary>
    /// Identifies new demographics that don't exist in the database.
    /// </summary>
    /// <param name="incoming">List of incoming demographics</param>
    /// <param name="existing">List of existing demographics</param>
    /// <returns>List of new demographics to insert</returns>
    List<Demographic> IdentifyNewDemographics(
        List<Demographic> incoming,
        List<Demographic> existing);
}
