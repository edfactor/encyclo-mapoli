using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Metrics;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Data.Repositories;
using Demoulas.ProfitSharing.OracleHcm.Commands;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Mappers;
using Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Demographics ingestion/upsert orchestrator that coordinates domain services for matching, auditing, and history tracking.
/// Simplified to use injected domain services for better testability.
/// </summary>
public sealed class DemographicsService : IDemographicsServiceInternal
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicsRepository _repository;
    private readonly IDemographicMatchingService _matchingService;
    private readonly IDemographicAuditService _auditService;
    private readonly IDemographicHistoryService _historyService;
    private readonly DemographicMapper _mapper;
    private readonly ILogger<DemographicsService> _logger;
    private readonly ITotalService _totalService;
    private readonly IFakeSsnService _fakeSsnService;

    public DemographicsService(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicsRepository repository,
        IDemographicMatchingService matchingService,
        IDemographicAuditService auditService,
        IDemographicHistoryService historyService,
        DemographicMapper mapper,
        ILogger<DemographicsService> logger,
        ITotalService totalService,
        IFakeSsnService fakeSsnService)
    {
        _dataContextFactory = dataContextFactory;
        _repository = repository;
        _matchingService = matchingService;
        _auditService = auditService;
        _historyService = historyService;
        _mapper = mapper;
        _logger = logger;
        _totalService = totalService;
        _fakeSsnService = fakeSsnService;
    }

    public async Task AddDemographicsStreamAsync(DemographicsRequest[] employees, byte batchSize = byte.MaxValue, CancellationToken cancellationToken = default)
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
            incomingByOracleId, cancellationToken).ConfigureAwait(false);

        HashSet<long> matchedOracleIds = existingByPrimary.Select(e => e.OracleHcmId).ToHashSet();

        // Step 2: Fallback matching by (SSN, BadgeNumber)
        List<(int Ssn, int BadgeNumber)> fallbackPairs = incoming
            .Where(e => !matchedOracleIds.Contains(e.OracleHcmId))
            .Select(e => (e.Ssn, e.BadgeNumber))
            .Distinct()
            .ToList();

        (List<Demographic> existingByFallback, bool skippedAllZeroBadge) =
            await _matchingService.MatchByFallbackAsync(fallbackPairs, cancellationToken).ConfigureAwait(false);

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

            throw;
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
            await AddDemographicSyncAuditAsync(targetDemographic, $"Merging ProfitDetails from source OracleHcmId {sourceDemographic.OracleHcmId} to OracleHcmId {targetDemographic.OracleHcmId}", "ProfitDetails", context, cancellationToken).ConfigureAwait(false);
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

    #region Private Helpers

    private async Task HandleExistingEmployeeMatchToProposedSsnAsync(Demographic existingEmployeeMatchToProposedSsn, Demographic demographicMarkedForSsnChange, ProfitSharingDbContext context, CancellationToken cancellationToken)
    {
        var result = await _totalService.GetVestingBalanceForSingleMemberAsync(SearchBy.Ssn, demographicMarkedForSsnChange.Ssn, (short)DateTime.Now.Year, cancellationToken).ConfigureAwait(false);
        if (existingEmployeeMatchToProposedSsn.EmploymentStatusId == EmploymentStatus.Constants.Terminated && result?.CurrentBalance == 0.0m)
        {
            await AddDemographicSyncAuditAsync(demographicMarkedForSsnChange, "Duplicate SSN found for terminated user with zero balance.", "SSN", context, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await AddDemographicSyncAuditAsync(demographicMarkedForSsnChange, "Duplicate SSN added for user.", "SSN", context, cancellationToken).ConfigureAwait(false);
        }
    }

    private static void AddDemographicSyncAudits(List<Demographic> duplicateSsnEntities, string message, string property, ProfitSharingDbContext context)
    {
        List<DemographicSyncAudit> audit = duplicateSsnEntities.Select(d => new DemographicSyncAudit
        {
            BadgeNumber = d.BadgeNumber,
            OracleHcmId = d.OracleHcmId,
            InvalidValue = d.Ssn.MaskSsn(),
            Message = message,
            UserName = Constants.SystemAccountName,
            PropertyName = property
        }).ToList();
        context.DemographicSyncAudit.AddRange(audit);
    }

    private static async Task AddDemographicSyncAuditAsync(Demographic demographic, string message, string property, ProfitSharingDbContext context, CancellationToken cancellationToken)
    {
        var audit = new DemographicSyncAudit
        {
            BadgeNumber = demographic.BadgeNumber,
            OracleHcmId = demographic.OracleHcmId,
            InvalidValue = demographic.Ssn.MaskSsn(),
            Message = message,
            UserName = Constants.SystemAccountName,
            PropertyName = property
        };
        await context.DemographicSyncAudit.AddAsync(audit, cancellationToken).ConfigureAwait(false);
    }

    private static void UpdateEntityValues(Demographic existingEntity, Demographic incomingEntity, DateTimeOffset modificationDate)
    {
        existingEntity.Ssn = incomingEntity.Ssn;
        existingEntity.BadgeNumber = incomingEntity.BadgeNumber;
        existingEntity.StoreNumber = incomingEntity.StoreNumber;
        existingEntity.DepartmentId = incomingEntity.DepartmentId;
        existingEntity.PayClassificationId = incomingEntity.PayClassificationId;
        existingEntity.ContactInfo = incomingEntity.ContactInfo;
        existingEntity.Address = incomingEntity.Address;
        existingEntity.DateOfBirth = incomingEntity.DateOfBirth;
        existingEntity.FullTimeDate = incomingEntity.FullTimeDate;
        existingEntity.HireDate = incomingEntity.HireDate;
        existingEntity.ReHireDate = incomingEntity.ReHireDate;
        existingEntity.TerminationCodeId = incomingEntity.TerminationCodeId;
        existingEntity.TerminationDate = incomingEntity.TerminationDate;
        existingEntity.EmploymentTypeId = incomingEntity.EmploymentTypeId;
        existingEntity.PayFrequencyId = incomingEntity.PayFrequencyId;
        existingEntity.GenderId = incomingEntity.GenderId;
        existingEntity.EmploymentStatusId = incomingEntity.EmploymentStatusId;
        existingEntity.ModifiedAtUtc = modificationDate;
    }

    private static async Task<int> UpdateExistingDemographicsAsync(Dictionary<long, Demographic> byOracle, ILookup<(int Ssn, int Badge), Demographic> bySsnBadge, List<Demographic> existing, DateTimeOffset modification, ProfitSharingDbContext context, CancellationToken ct)
    {
        int updated = 0;
        foreach (var existingEntity in existing)
        {
            Demographic? incoming = byOracle.TryGetValue(existingEntity.OracleHcmId, out var found) ? found : bySsnBadge[(existingEntity.Ssn, existingEntity.BadgeNumber)].FirstOrDefault();
            if (incoming == null)
            {
                continue;
            }

            if (existingEntity.OracleHcmId != incoming.OracleHcmId && existingEntity.OracleHcmId < int.MaxValue)
            {
                existingEntity.OracleHcmId = incoming.OracleHcmId;
            }

            bool changed = !Demographic.DemographicHistoryEqual(existingEntity, incoming);
            if (existingEntity.Ssn != incoming.Ssn)
            {
                var beneficiaries = context.BeneficiaryContacts.Where(b => b.Ssn == existingEntity.Ssn);
                await beneficiaries.ForEachAsync(b => b.Ssn = incoming.Ssn, ct).ConfigureAwait(false);
                var profitDetails = context.ProfitDetails.Where(p => p.Ssn == existingEntity.Ssn);
                await profitDetails.ForEachAsync(p => p.Ssn = incoming.Ssn, ct).ConfigureAwait(false);
            }

            UpdateEntityValues(existingEntity, incoming, modification);
            if (changed)
            {
                DemographicHistory newHistory = DemographicHistory.FromDemographic(incoming, existingEntity.Id);
                DemographicHistory oldHistory = await context.DemographicHistories
                    .TagWith($"DemographicsSync-GetCurrentHistory-DemographicId:{existingEntity.Id}")
                    .Where(h => h.DemographicId == existingEntity.Id && DateTimeOffset.UtcNow >= h.ValidFrom && DateTimeOffset.UtcNow < h.ValidTo)
                    .FirstAsync(ct).ConfigureAwait(false);
                oldHistory.ValidTo = DateTimeOffset.UtcNow;
                newHistory.ValidFrom = oldHistory.ValidTo;
                context.DemographicHistories.Add(newHistory);
            }
            updated++;
        }
        return updated;
    }

    private async Task<int> InsertNewDemographicsAsync(List<Demographic> incoming, List<Demographic> existing, ProfitSharingDbContext context, CancellationToken ct)
    {
        HashSet<long> existingOracleIds = existing.Select(e => e.OracleHcmId).ToHashSet();
        HashSet<(int Ssn, int Badge)> existingSsnBadges = existing.Select(e => (e.Ssn, e.BadgeNumber)).ToHashSet();
        List<Demographic> newEntities = incoming
            .Where(e => !existingOracleIds.Contains(e.OracleHcmId) && !existingSsnBadges.Contains((e.Ssn, e.BadgeNumber)))
            .ToList();
        int inserted = 0;
        foreach (var entity in newEntities)
        {
            try
            {
                context.Demographics.Add(entity);
                context.DemographicHistories.Add(DemographicHistory.FromDemographic(entity));
                inserted++;
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("When attaching existing entities"))
            {
                try { await context.SaveChangesAsync(ct).ConfigureAwait(false); } catch (Exception inner) { _logger.LogCritical(inner, "Failed partial save during insert error"); }
            }
        }
        return inserted;
    }

    private static async Task<List<Demographic>> GetMatchingDemographicsByFallbackSqlAsync(List<(int Ssn, int Badge)> pairs, ProfitSharingDbContext context, CancellationToken ct)
    {
        List<string> clauses = new();
        List<OracleParameter> parameters = new();
        int i = 0;
        foreach (var (ssn, badge) in pairs)
        {
            if (badge == 0)
            {
                continue;
            }
            string pSsn = $":p{i}"; parameters.Add(new OracleParameter($"p{i}", ssn)); i++;
            string pBadge = $":p{i}"; parameters.Add(new OracleParameter($"p{i}", badge)); i++;
            clauses.Add($"(d.SSN = {pSsn} AND d.BADGE_NUMBER = {pBadge})");
        }
        if (clauses.Count == 0)
        {
            return new List<Demographic>();
        }
        string sql = $"SELECT * FROM DEMOGRAPHIC WHERE {string.Join(" OR ", clauses)}";
        return await context.Demographics
            .FromSqlRaw(sql, parameters.ToArray())
            .TagWith($"DemographicsSync-FallbackMatch-SSNBadge-Count:{pairs.Count}")
            .Include(d => d.ContactInfo)
            .Include(d => d.Address)
            .ToListAsync(ct).ConfigureAwait(false);
    }

    #endregion
}
