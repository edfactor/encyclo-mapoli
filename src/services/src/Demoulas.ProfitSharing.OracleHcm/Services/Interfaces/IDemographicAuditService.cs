using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;

/// <summary>
/// Service for detecting and auditing demographic data issues.
/// Handles duplicate SSN detection, SSN conflict resolution, and audit logging.
/// </summary>
public interface IDemographicAuditService
{
    /// <summary>
    /// Detects demographics with duplicate SSNs.
    /// </summary>
    /// <param name="existing">List of existing demographics to check</param>
    /// <param name="fallbackSsns">List of SSNs from fallback matching</param>
    /// <returns>List of demographics with duplicate SSNs</returns>
    List<Demographic> DetectDuplicateSsns(
        List<Demographic> existing,
        List<int> fallbackSsns);

    /// <summary>
    /// Creates audit records for duplicate SSNs.
    /// </summary>
    /// <param name="duplicates">List of demographics with duplicate SSNs</param>
    /// <param name="ct">Cancellation token</param>
    Task AuditDuplicateSsnsAsync(
        List<Demographic> duplicates,
        CancellationToken ct);

    /// <summary>
    /// Checks for SSN conflicts and creates appropriate audit records.
    /// Handles cases where OracleHcmId matches but SSN differs.
    /// </summary>
    /// <param name="incomingByOracleId">Dictionary of incoming demographics by OracleHcmId</param>
    /// <param name="existing">List of existing demographics</param>
    /// <param name="ct">Cancellation token</param>
    Task CheckSsnConflictsAsync(
        Dictionary<long, Demographic> incomingByOracleId,
        List<Demographic> existing,
        CancellationToken ct);

    /// <summary>
    /// Creates a single audit record for a demographic.
    /// </summary>
    /// <param name="demographic">Demographic to audit</param>
    /// <param name="message">Audit message</param>
    /// <param name="property">Property name being audited</param>
    /// <param name="ct">Cancellation token</param>
    Task CreateAuditAsync(
        Demographic demographic,
        string message,
        string property,
        CancellationToken ct);
}
