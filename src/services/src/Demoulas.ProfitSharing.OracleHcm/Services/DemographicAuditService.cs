using Demoulas.ProfitSharing.Common.Metrics;
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
            DemographicsIngestMetrics.DuplicateSsns.Add(duplicateSsnGroups.Count);
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

            _logger.LogCritical(
                "Multiple demographics with same SSN. " +
                "SSN={SSN}, OracleHcmIds={OracleHcmIds}, BadgeNumbers={BadgeNumbers}",
                group.Key, ids, badges);

            var auditRecord = new DemographicsAudit
            {
                AuditedAt = DateTime.UtcNow,
                RecordType = "DuplicateSSN",
                RecordValue = group.Key.ToString(),
                Detail = $"OracleHcmIds: {string.Join(", ", ids)}; Badges: {string.Join(", ", badges)}",
                DemographicId = group.First().Id
            };

            commands.Add(new AddAuditCommand(auditRecord));
        }

        if (duplicateGroups.Count > 0)
        {
            DemographicsIngestMetrics.DuplicateAudits.Add(duplicateGroups.Count);
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

        DemographicsIngestMetrics.SsnConflicts.Add(conflictingItems.Count);

        foreach (var incomingItem in conflictingItems)
        {
            var conflictingExisting = existingBySsn[incomingItem.Ssn]
                .Where(e => e.BadgeNumber != incomingItem.BadgeNumber || e.OracleHcmId != incomingItem.OracleHcmId)
                .ToList();

            foreach (var existingItem in conflictingExisting)
            {
                // If existing record is terminated, allow overwrite
                if (existingItem.EmploymentStatus == "T")
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

                var auditRecord = new DemographicsAudit
                {
                    AuditedAt = DateTime.UtcNow,
                    RecordType = "SSNConflict",
                    RecordValue = incomingItem.Ssn.ToString(),
                    Detail = $"Existing Badge={existingItem.BadgeNumber}, OracleId={existingItem.OracleHcmId}; " +
                             $"Incoming Badge={incomingItem.BadgeNumber}, OracleId={incomingItem.OracleHcmId}",
                    DemographicId = existingItem.Id
                };

                commands.Add(new AddAuditCommand(auditRecord));
            }
        }

        return commands;
    }

}
