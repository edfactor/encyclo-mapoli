using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.OracleHcm.Commands;

namespace Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;

/// <summary>
/// Service for detecting and auditing demographic data issues.
/// Returns commands for transaction-safe execution.
/// </summary>
public interface IDemographicAuditService
{
    /// <summary>
    /// Detects demographics with duplicate SSNs.
    /// </summary>
    /// <param name="demographics">List of demographics to check for duplicates</param>
    /// <returns>List of groupings by SSN that have duplicates</returns>
    List<IGrouping<int, Demographic>> DetectDuplicateSsns(
        List<Demographic> demographics);

    /// <summary>
    /// Prepares audit commands for duplicate SSNs.
    /// </summary>
    /// <param name="duplicateGroups">Groups of demographics with duplicate SSNs</param>
    /// <returns>List of commands to execute</returns>
    List<IDemographicCommand> PrepareAuditDuplicateSsns(
        List<IGrouping<int, Demographic>> duplicateGroups);

    /// <summary>
    /// Prepares audit commands for SSN conflicts.
    /// Handles cases where SSN matches but Badge or OracleId differ.
    /// </summary>
    /// <param name="existing">List of existing demographics</param>
    /// <param name="incoming">List of incoming demographics</param>
    /// <returns>List of commands to execute</returns>
    List<IDemographicCommand> PrepareCheckSsnConflicts(
        List<Demographic> existing,
        List<Demographic> incoming);
}
