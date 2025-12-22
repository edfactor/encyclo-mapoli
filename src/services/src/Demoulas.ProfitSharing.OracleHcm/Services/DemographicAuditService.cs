using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.OracleHcm.Commands;
using Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Handles audit operations and duplicate detection for demographics.
/// Returns commands for transaction-safe execution.
/// </summary>
public sealed class DemographicAuditService : IDemographicAuditService
{
    private readonly ILogger<DemographicAuditService> _logger;

    public DemographicAuditService(ILogger<DemographicAuditService> logger)
    {
        _logger = logger;
    }

    public List<IGrouping<int, Demographic>> DetectDuplicateSsns(List<Demographic> demographics)
    {
        var duplicateSsnGroups = demographics
            .GroupBy(e => e.Ssn)
            .Where(g => g.Count() > 1)
            .ToList();

        if (duplicateSsnGroups.Count > 0)
        {
            _logger.LogDebug("Detected {Count} duplicate SSN groups", duplicateSsnGroups.Count);
        }

        return duplicateSsnGroups;
    }

    public List<IDemographicCommand> PrepareAuditDuplicateSsns(
        List<IGrouping<int, Demographic>> duplicateGroups)
    {
        var commands = new List<IDemographicCommand>();

        foreach (var group in duplicateGroups)
        {
            var ids = group.Select(e => e.OracleHcmId).ToList();
            var badges = group.Select(e => e.BadgeNumber).ToList();

            _logger.LogWarning(
                "Duplicate SSN detected: " +
                "SSN={SSN}, OracleHcmIds={OracleHcmIds}, BadgeNumbers={BadgeNumbers}",
                group.Key, ids, badges);

            var auditRecord = new DemographicSyncAudit
            {
                BadgeNumber = group.First().BadgeNumber,
                OracleHcmId = group.First().OracleHcmId,
                PropertyName = "SSN",
                Message = $"Duplicate SSN detected: {group.Key}. OracleHcmIds: {string.Join(", ", ids)}; Badges: {string.Join(", ", badges)}"
            };

            commands.Add(new AddAuditCommand(auditRecord));
        }

        return commands;
    }
    public List<IDemographicCommand> PrepareCheckSsnConflicts(
        List<Demographic> existing,
        List<Demographic> incoming)
    {
        var commands = new List<IDemographicCommand>();
        var existingBySsn = existing.ToLookup(e => e.Ssn);

        var conflictingItems = incoming
            .Where(incoming =>
            {
                var existingWithSameSsn = existingBySsn[incoming.Ssn].ToList();
                if (existingWithSameSsn.Count == 0)
                {
                    return false;
                }

                // Conflict if SSN matches but Badge or OracleId differ
                return existingWithSameSsn.Any(existing =>
                    existing.BadgeNumber != incoming.BadgeNumber ||
                    existing.OracleHcmId != incoming.OracleHcmId);
            })
            .ToList();

        if (conflictingItems.Count == 0)
        {
            return commands;
        }

        foreach (var incomingItem in conflictingItems)
        {
            var conflictingExisting = existingBySsn[incomingItem.Ssn]
                .Where(e => e.BadgeNumber != incomingItem.BadgeNumber || e.OracleHcmId != incomingItem.OracleHcmId)
                .ToList();

            foreach (var existingItem in conflictingExisting)
            {
                // If existing record is terminated, allow overwrite
                if (existingItem.EmploymentStatusId == EmploymentStatus.Constants.Terminated)
                {
                    _logger.LogWarning(
                        "SSN conflict detected but existing record is terminated. " +
                        "SSN={SSN}, ExistingBadge={ExistingBadge}, IncomingBadge={IncomingBadge}, " +
                        "ExistingOracleId={ExistingOracleId}, IncomingOracleId={IncomingOracleId}",
                        incomingItem.Ssn, existingItem.BadgeNumber, incomingItem.BadgeNumber,
                        existingItem.OracleHcmId, incomingItem.OracleHcmId);
                    continue;
                }

                _logger.LogCritical(
                    "SSN conflict detected with non-terminated record. " +
                    "SSN={SSN}, ExistingBadge={ExistingBadge}, IncomingBadge={IncomingBadge}, " +
                    "ExistingOracleId={ExistingOracleId}, IncomingOracleId={IncomingOracleId}",
                    incomingItem.Ssn, existingItem.BadgeNumber, incomingItem.BadgeNumber,
                    existingItem.OracleHcmId, incomingItem.OracleHcmId);

                var auditRecord = new DemographicSyncAudit
                {
                    BadgeNumber = existingItem.BadgeNumber,
                    OracleHcmId = existingItem.OracleHcmId,
                    PropertyName = "SSN",
                    Message = $"SSN conflict: SSN={incomingItem.Ssn}. " +
                              $"Existing Badge={existingItem.BadgeNumber}, OracleId={existingItem.OracleHcmId}; " +
                              $"Incoming Badge={incomingItem.BadgeNumber}, OracleId={incomingItem.OracleHcmId}"
                };

                commands.Add(new AddAuditCommand(auditRecord));
            }
        }

        return commands;
    }

}
