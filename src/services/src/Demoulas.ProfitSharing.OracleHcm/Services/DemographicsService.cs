using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Metrics;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Commands;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Mappers;
using Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Demographics ingestion/upsert orchestrator that coordinates domain services for matching, auditing, and history tracking.
/// Simplified to use injected domain services for better testability.
/// </summary>
public sealed class DemographicsService : IDemographicsServiceInternal
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicMatchingService _matchingService;
    private readonly IDemographicAuditService _auditService;
    private readonly IDemographicHistoryService _historyService;
    private readonly DemographicMapper _mapper;
    private readonly ILogger<DemographicsService> _logger;
    private readonly IFakeSsnService _fakeSsnService;

    public DemographicsService(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicsRepository repository,
        IDemographicMatchingService matchingService,
        IDemographicAuditService auditService,
        IDemographicHistoryService historyService,
        DemographicMapper mapper,
        ILogger<DemographicsService> logger,
        IFakeSsnService fakeSsnService)
    {
        _dataContextFactory = dataContextFactory;
        _matchingService = matchingService;
        _auditService = auditService;
        _historyService = historyService;
        _mapper = mapper;
        _logger = logger;
        _fakeSsnService = fakeSsnService;
    }

    public async Task AddDemographicsStreamAsync(DemographicsRequest[] employees, ushort batchSize = byte.MaxValue, CancellationToken cancellationToken = default)
    {
        DemographicsIngestMetrics.EnsureInitialized();
        long startTicks = Environment.TickCount64;
        DateTimeOffset modification = DateTimeOffset.UtcNow;

        // Map incoming requests to domain entities
        List<Demographic> incoming = _mapper.Map(employees).ToList();
        incoming.ForEach(e => e.ModifiedAtUtc = modification);

        int requested = incoming.Count;
        DemographicsIngestMetrics.BatchesTotal.Add(1);
        DemographicsIngestMetrics.RecordsRequested.Add(requested);

        // Build lookup structures
        Dictionary<long, Demographic> incomingByOracleId = incoming.ToDictionary(e => e.OracleHcmId);

        // Step 1: Primary matching by OracleHcmId
        List<Demographic> existingByPrimary = await _matchingService.MatchByOracleIdAsync(
            incomingByOracleId, cancellationToken).ConfigureAwait(false) ?? [];

        HashSet<long> matchedOracleIds = existingByPrimary.Select(e => e.OracleHcmId).ToHashSet();

        // Step 2: Fallback matching by (SSN, BadgeNumber)
        List<(int Ssn, int BadgeNumber)> fallbackPairs = incoming
            .Where(e => !matchedOracleIds.Contains(e.OracleHcmId))
            .Select(e => (e.Ssn, e.BadgeNumber))
            .Distinct()
            .ToList();

        var fallbackResult = await _matchingService.MatchByFallbackAsync(fallbackPairs, cancellationToken).ConfigureAwait(false);
        List<Demographic> existingByFallback = fallbackResult.Matched ?? [];
        bool skippedAllZeroBadge = fallbackResult.SkippedAllZeroBadge;

        // Combine all existing records (deduplicate by Id)
        List<Demographic> existing = existingByPrimary
            .Concat(existingByFallback)
            .GroupBy(e => e.Id)
            .Select(g => g.First())
            .ToList();

        // Step 3: Collect commands from domain services
        var commands = new List<IDemographicCommand>();

        // 3a: Detect duplicate SSNs and prepare audit commands
        var duplicateSsnGroups = _auditService.DetectDuplicateSsns(existing);
        if (duplicateSsnGroups.Count > 0)
        {
            var auditCommands = _auditService.PrepareAuditDuplicateSsns(duplicateSsnGroups);
            commands.AddRange(auditCommands);
        }

        // 3b: Check for SSN conflicts and prepare audit commands
        var conflictCommands = _auditService.PrepareCheckSsnConflicts(existing, incoming);
        commands.AddRange(conflictCommands);

        // 3c: Identify new demographics and prepare insert commands with history
        List<Demographic> newDemographics = _matchingService.IdentifyNewDemographics(incoming, existing);
        var (inserted, insertCommands) = _historyService.PrepareInsertNewWithHistory(newDemographics);
        commands.AddRange(insertCommands);
        if (inserted > 0)
        {
            DemographicsIngestMetrics.RecordsInserted.Add(inserted);
        }

        // 3d: Prepare update commands with history tracking
        var (updated, updateCommands) = _historyService.PrepareUpdateExistingWithHistory(existing, incoming);
        commands.AddRange(updateCommands);
        if (updated > 0)
        {
            DemographicsIngestMetrics.RecordsUpdated.Add(updated);
        }

        // 3e: Detect SSN changes and prepare update commands for related entities
        List<Demographic> ssnChangedDemographics = _historyService.DetectSsnChanges(existing, incomingByOracleId);
        if (ssnChangedDemographics.Count > 0)
        {
            var ssnUpdateCommands = _historyService.PrepareSsnUpdateCommands(ssnChangedDemographics, incomingByOracleId);
            commands.AddRange(ssnUpdateCommands);
        }

        // Step 4: Execute all commands in single transaction
        try
        {
            await _dataContextFactory.UseWritableContext(async context =>
            {
                foreach (var command in commands)
                {
                    await command.ExecuteAsync(context, cancellationToken).ConfigureAwait(false);
                }
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);

            // Record successful business operation
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "demographics-ingest-batch"),
                new("status", "success"),
                new("service", nameof(DemographicsService)));

            EndpointTelemetry.RecordCountsProcessed.Record(requested,
                new("operation", "demographics-ingest"),
                new("record.type", "demographics-requested"),
                new("service", nameof(DemographicsService)));
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to save batch: {DemographicsRequests}", employees);
            DemographicsIngestMetrics.BatchFailures.Add(1);

            // Record failure
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "demographics-ingest-batch"),
                new("status", "failed"),
                new("service", nameof(DemographicsService)));

            EndpointTelemetry.EndpointErrorsTotal.Add(1,
                new("error.type", ex.GetType().Name),
                new("operation", "demographics-ingest-batch"),
                new("service", nameof(DemographicsService)));

            throw new InvalidOperationException($"Failed to save demographics batch with {requested} requested records. See inner exception for details.", ex);
        }

        // Log summary
        _logger.LogInformation("Demographics ingest batch summary {@Metrics}", new
        {
            Requested = requested,
            PrimaryMatched = existingByPrimary.Count,
            FallbackPairs = fallbackPairs.Count,
            FallbackMatched = existingByFallback.Count,
            Inserted = inserted,
            Updated = updated,
            SkippedAllZeroBadgeFallback = skippedAllZeroBadge
        });

        double durationMs = (Environment.TickCount64 - startTicks);
        DemographicsIngestMetrics.BatchDurationMs.Record(durationMs);
    }

    public Task MergeProfitDetailsToDemographic(Demographic sourceDemographic, Demographic targetDemographic, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseWritableContext(async context =>
        {
            var fakeSsn = await _fakeSsnService.GenerateFakeSsnAsync(cancellationToken).ConfigureAwait(false);
            sourceDemographic.Ssn = fakeSsn;

            // Create audit record using command pattern
            var auditRecord = new DemographicSyncAudit
            {
                BadgeNumber = targetDemographic.BadgeNumber,
                OracleHcmId = targetDemographic.OracleHcmId,
                InvalidValue = sourceDemographic.OracleHcmId.ToString(),
                UserName = Constants.SystemAccountName,
                PropertyName = "ProfitDetails",
                Message = $"Merging ProfitDetails from source OracleHcmId {sourceDemographic.OracleHcmId} to OracleHcmId {targetDemographic.OracleHcmId}"
            };

            var auditCommand = new AddAuditCommand(auditRecord);
            await auditCommand.ExecuteAsync(context, cancellationToken).ConfigureAwait(false);

            try
            {
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Failed to merge demographics: Source SSN {SourceSsn}, Target SSN {TargetSsn}", targetDemographic.Ssn.MaskSsn(), targetDemographic.Ssn.MaskSsn());
            }
        }, cancellationToken);
    }

    public Task AuditError(int badgeNumber, long oracleHcmId, IEnumerable<ValidationFailure> errorMessages, string requestedBy, CancellationToken cancellationToken = default, params object?[] args)
    {
        return _dataContextFactory.UseWritableContext(c =>
        {
            for (int i = 0; i < args?.Length; i++)
            {
                args[i] ??= "null";
            }
            IEnumerable<DemographicSyncAudit> auditRecords = errorMessages.Select(e => new DemographicSyncAudit
            {
                BadgeNumber = badgeNumber,
                OracleHcmId = oracleHcmId,
                InvalidValue = e.AttemptedValue?.ToString() ?? e.CustomState?.ToString(),
                Message = e.ErrorMessage,
                UserName = requestedBy,
                PropertyName = e.PropertyName,
            });
            c.DemographicSyncAudit.AddRange(auditRecords);
            return c.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }

    public Task CleanAuditError(CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseWritableContext(c =>
        {
            DateTime clearBackTo = DateTime.Today.AddDays(-30);
            return c.DemographicSyncAudit.Where(t => t.Created < clearBackTo).ExecuteDeleteAsync(cancellationToken);
        }, cancellationToken);
    }
}
