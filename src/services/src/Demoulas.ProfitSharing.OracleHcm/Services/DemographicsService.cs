using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Mappers;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Common.Metrics;
using EntityFramework.Exceptions.Common;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Demographics ingestion/upsert with primary OracleHcmId matching, (SSN,BadgeNumber) fallback, history tracking, and audit logging.
/// Includes metrics counters & structured summary logging.
/// </summary>
public class DemographicsService : IDemographicsServiceInternal
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly DemographicMapper _mapper;
    private readonly ILogger<DemographicsService> _logger;
    private readonly ITotalService _totalService;
    private readonly IFakeSsnService _fakeSsnService;

    public DemographicsService(IProfitSharingDataContextFactory dataContextFactory, DemographicMapper mapper, ILogger<DemographicsService> logger, ITotalService totalService, IFakeSsnService fakeSsnService)
    {
        _dataContextFactory = dataContextFactory;
        _mapper = mapper;
        _logger = logger;
        _totalService = totalService;
        _fakeSsnService = fakeSsnService;
    }

    public Task AddDemographicsStreamAsync(DemographicsRequest[] employees, byte batchSize = byte.MaxValue, CancellationToken cancellationToken = default)
    {
        DemographicsIngestMetrics.EnsureInitialized();
        long startTicks = Environment.TickCount64;
        DateTimeOffset modification = DateTimeOffset.UtcNow;
        List<Demographic> incoming = _mapper.Map(employees).ToList();
        incoming.ForEach(e => e.ModifiedAtUtc = modification);

        Dictionary<long, Demographic> byOracle = incoming.ToDictionary(e => e.OracleHcmId);
        ILookup<(int Ssn, int Badge), Demographic> bySsnBadge = incoming.ToLookup(e => (e.Ssn, e.BadgeNumber));

        int requested = incoming.Count;
        int primaryMatched = 0;
        int fallbackPairsCount = 0;
        int fallbackMatched = 0;
        int inserted = 0;
        int updated = 0;
        bool skippedAllZeroBadgeFallback = false;

        return _dataContextFactory.UseWritableContext(async context =>
        {
            DemographicsIngestMetrics.BatchesTotal.Add(1);
            DemographicsIngestMetrics.RecordsRequested.Add(requested);
            // Primary match by OracleHcmId
            List<long> oracleIds = byOracle.Keys.ToList();
            List<Demographic> existingByOracle = await context.Demographics
                .Where(d => oracleIds.Contains(d.OracleHcmId))
                .Include(d => d.ContactInfo)
                .Include(d => d.Address)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            primaryMatched = existingByOracle.Count;
            if (primaryMatched > 0)
            {
                DemographicsIngestMetrics.PrimaryMatched.Add(primaryMatched);
            }
            HashSet<long> matchedOracleIds = existingByOracle.Select(e => e.OracleHcmId).ToHashSet();

            // Fallback (SSN,Badge)
            List<(int Ssn, int Badge)> fallbackPairs = incoming
                .Where(e => !matchedOracleIds.Contains(e.OracleHcmId))
                .Select(e => (e.Ssn, e.BadgeNumber))
                .Distinct()
                .ToList();
            fallbackPairsCount = fallbackPairs.Count;
            if (fallbackPairsCount > 0)
            {
                DemographicsIngestMetrics.FallbackPairs.Add(fallbackPairsCount);
            }

            List<Demographic> existingByFallback = new();
            if (fallbackPairs.Count > 0)
            {
                if (fallbackPairs.All(p => p.Badge == 0))
                {
                    skippedAllZeroBadgeFallback = true;
                    _logger.LogCritical("All fallback demographic pairs have BadgeNumber == 0. Aborting fallback lookup. OracleMatched={OracleMatched} FallbackPairs={FallbackPairs}", primaryMatched, fallbackPairs.Count);
                    DemographicsIngestMetrics.FallbackSkippedAllZeroBadge.Add(1);
                }
                else
                {
                    existingByFallback = await GetMatchingDemographicsByFallbackSqlAsync(fallbackPairs, context, cancellationToken).ConfigureAwait(false);
                    fallbackMatched = existingByFallback.Count;
                    if (fallbackMatched > 0)
                    {
                        DemographicsIngestMetrics.FallbackMatched.Add(fallbackMatched);
                    }
                }
            }

            List<Demographic> existing = existingByOracle
                .Concat(existingByFallback)
                .GroupBy(e => e.Id)
                .Select(g => g.First())
                .ToList();

            // Duplicate SSN audit
            List<Demographic> duplicateSsns = existing
                .GroupBy(e => e.Ssn)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .ToList();
            if (duplicateSsns.Any())
            {
                AddDemographicSyncAudits(duplicateSsns, "Duplicate SSNs found in the database.", "SSN", context);
            }
            else
            {
                foreach (var kv in byOracle)
                {
                    var existingEntity = existing.FirstOrDefault(db => db.OracleHcmId == kv.Key && db.Ssn != kv.Value.Ssn);
                    if (existingEntity == null)
                    {
                        continue;
                    }
                    var proposed = kv.Value;
                    Demographic? matchToProposed = await context.Demographics.FirstOrDefaultAsync(d => d.Ssn == proposed.Ssn, cancellationToken).ConfigureAwait(false);
                    if (matchToProposed == null)
                    {
                        await AddDemographicSyncAuditAsync(existingEntity, "SSN changed with no conflicts.", "SSN", context, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        await HandleExistingEmployeeMatchToProposedSsnAsync(matchToProposed, existingEntity, context, cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            inserted = await InsertNewDemographicsAsync(incoming, existing, context, cancellationToken).ConfigureAwait(false);
            updated = await UpdateExistingDemographicsAsync(byOracle, bySsnBadge, existing, modification, context, cancellationToken).ConfigureAwait(false);
            if (inserted > 0) { DemographicsIngestMetrics.RecordsInserted.Add(inserted); }
            if (updated > 0) { DemographicsIngestMetrics.RecordsUpdated.Add(updated); }

            try
            {
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to save batch: {DemographicsRequests}", employees);
                DemographicsIngestMetrics.BatchFailures.Add(1);
            }

            _logger.LogInformation("Demographics ingest batch summary {@Metrics}", new
            {
                Requested = requested,
                PrimaryMatched = primaryMatched,
                FallbackPairs = fallbackPairsCount,
                FallbackMatched = fallbackMatched,
                Inserted = inserted,
                Updated = updated,
                SkippedAllZeroBadgeFallback = skippedAllZeroBadgeFallback
            });
            double durationMs = (Environment.TickCount64 - startTicks);
            DemographicsIngestMetrics.BatchDurationMs.Record(durationMs);
        }, cancellationToken);
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

    protected virtual async Task<List<Demographic>> GetMatchingDemographicsByFallbackSqlAsync(List<(int Ssn, int Badge)> pairs, ProfitSharingDbContext context, CancellationToken ct)
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
        string sql = $"SELECT d.* FROM DEMOGRAPHIC d WHERE {string.Join(" OR ", clauses)}";
        return await context.Demographics
            .FromSqlRaw(sql, parameters.ToArray())
            .Include(d => d.ContactInfo)
            .Include(d => d.Address)
            .ToListAsync(ct).ConfigureAwait(false);
    }

    #endregion
}
