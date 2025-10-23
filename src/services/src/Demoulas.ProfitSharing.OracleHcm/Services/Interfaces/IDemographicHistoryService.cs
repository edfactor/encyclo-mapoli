using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;

/// <summary>
/// Service for managing demographic history tracking and entity updates.
/// Handles closing old history records and creating new ones when data changes.
/// </summary>
public interface IDemographicHistoryService
{
    /// <summary>
    /// Updates existing demographics with incoming data and tracks history changes.
    /// Creates new history records when data changes are detected.
    /// </summary>
    /// <param name="incomingByOracleId">Dictionary of incoming demographics by OracleHcmId</param>
    /// <param name="incomingBySsnBadge">Lookup of incoming demographics by (SSN, BadgeNumber)</param>
    /// <param name="existing">List of existing demographics to update</param>
    /// <param name="modificationDate">Timestamp for the update</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Number of demographics updated</returns>
    Task<int> UpdateExistingWithHistoryAsync(
        Dictionary<long, Demographic> incomingByOracleId,
        ILookup<(int Ssn, int BadgeNumber), Demographic> incomingBySsnBadge,
        List<Demographic> existing,
        DateTimeOffset modificationDate,
        CancellationToken ct);

    /// <summary>
    /// Inserts new demographics with initial history records.
    /// </summary>
    /// <param name="newDemographics">List of new demographics to insert</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Number of demographics inserted</returns>
    Task<int> InsertNewWithHistoryAsync(
        List<Demographic> newDemographics,
        CancellationToken ct);

    /// <summary>
    /// Updates related entities when SSN changes (BeneficiaryContacts, ProfitDetails).
    /// </summary>
    /// <param name="oldSsn">Old SSN value</param>
    /// <param name="newSsn">New SSN value</param>
    /// <param name="ct">Cancellation token</param>
    Task UpdateRelatedEntitiesForSsnChangeAsync(
        int oldSsn,
        int newSsn,
        CancellationToken ct);
}
