using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.OracleHcm.Commands;

namespace Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;

/// <summary>
/// Service for managing demographic history tracking and entity updates.
/// Returns commands for transaction-safe execution.
/// </summary>
public interface IDemographicHistoryService
{
    /// <summary>
    /// Prepares commands to update existing demographics with history tracking.
    /// Creates history records before updating entity values.
    /// </summary>
    /// <param name="existing">Existing demographics to update</param>
    /// <param name="incoming">Incoming demographics with new values</param>
    /// <returns>Tuple of (updated count, list of commands to execute)</returns>
    (int UpdatedCount, List<IDemographicCommand> Commands) PrepareUpdateExistingWithHistory(
        List<Demographic> existing,
        List<Demographic> incoming);

    /// <summary>
    /// Prepares commands to insert new demographics with initial history records.
    /// </summary>
    /// <param name="newDemographics">New demographics to insert</param>
    /// <returns>Tuple of (inserted count, list of commands to execute)</returns>
    (int InsertedCount, List<IDemographicCommand> Commands) PrepareInsertNewWithHistory(
        List<Demographic> newDemographics);

    /// <summary>
    /// Detects SSN changes between existing and incoming demographics.
    /// </summary>
    /// <param name="existing">Existing demographics</param>
    /// <param name="incomingByOracleId">Dictionary of incoming demographics by OracleHcmId</param>
    /// <returns>List of demographics with SSN changes</returns>
    List<Demographic> DetectSsnChanges(
        List<Demographic> existing,
        Dictionary<long, Demographic> incomingByOracleId);

    /// <summary>
    /// Prepares commands to update SSNs in related entities (BeneficiaryContacts, ProfitDetails).
    /// </summary>
    /// <param name="ssnChangedDemographics">Demographics with SSN changes</param>
    /// <param name="incomingByOracleId">Dictionary of incoming demographics by OracleHcmId</param>
    /// <returns>List of commands to execute</returns>
    List<IDemographicCommand> PrepareSsnUpdateCommands(
        List<Demographic> ssnChangedDemographics,
        Dictionary<long, Demographic> incomingByOracleId);
}
