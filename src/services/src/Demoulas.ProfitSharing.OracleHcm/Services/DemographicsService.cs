using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Mappers;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using EntityFramework.Exceptions.Common;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using Exception = System.Exception;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Provides services for handling demographic data related to employees within the Oracle HCM system.
/// </summary>
/// <remarks>
/// This service is responsible for processing, auditing, and managing demographic information.
/// It integrates with data context factories, mappers, and logging to ensure efficient and reliable operations.
/// </remarks>
public class DemographicsService : IDemographicsServiceInternal
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly DemographicMapper _mapper;
    private readonly ILogger<DemographicsService> _logger;
    private readonly ITotalService _totalService;
    private readonly IFakeSsnService _fakeSsnService;

    public DemographicsService(IProfitSharingDataContextFactory dataContextFactory,
        DemographicMapper mapper,
        ILogger<DemographicsService> logger,
        ITotalService totalService,
        IFakeSsnService fakeSsnService)
    {
        _dataContextFactory = dataContextFactory;
        _mapper = mapper;
        _logger = logger;
        _totalService = totalService;
        _fakeSsnService = fakeSsnService;
    }

    /// <summary>
    /// Asynchronously processes a stream of demographic requests and batches them for upsert operations.
    /// </summary>
    /// <param name="employees">
    /// An asynchronous enumerable of <see cref="DemographicsRequest"/> objects representing the demographic data to be processed.
    /// </param>
    /// <param name="batchSize">
    /// The maximum number of requests to process in a single batch. Defaults to <see cref="byte.MaxValue"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method enqueues incoming demographic requests and processes them in batches. 
    /// Once the batch size is reached, the requests are upserted into the database.
    /// </remarks>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the operation is canceled via the provided <paramref name="cancellationToken"/>.
    /// </exception>
    public Task AddDemographicsStreamAsync(DemographicsRequest[] employees, byte batchSize = byte.MaxValue,
        CancellationToken cancellationToken = default)
    {

        DateTimeOffset currentModificationDate = DateTimeOffset.UtcNow;

        // Map incoming demographic requests to entity models
        List<Demographic> demographicsEntities = _mapper.Map(employees).ToList();

        // Update LastModifiedDate for all entities
        demographicsEntities.ForEach(entity => entity.ModifiedAtUtc = currentModificationDate);

        // Create lookup dictionaries for both OracleHcmId and SSN
        Dictionary<long, Demographic> demographicOracleHcmIdLookup = demographicsEntities.ToDictionary(entity => entity.OracleHcmId);
        ILookup<(int Ssn, int BadgeNumber), Demographic> demographicSsnLookup = demographicsEntities.ToLookup(entity => (entity.Ssn, BadgeNumber: entity.BadgeNumber));

        // Use writable context for the upsert operation
        return _dataContextFactory.UseWritableContext(async context =>
        {
            List<Demographic> existingEntities = await GetMatchingDemographicsWithSqlAsync(demographicsEntities, context, cancellationToken).ConfigureAwait(false);

            // Handle potential duplicates in the existing database (SSN duplicates)
            List<Demographic> duplicateSsnEntities = existingEntities.GroupBy(e => e.Ssn)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .ToList();

            if (duplicateSsnEntities.Any())
            {
                AddDemographicSyncAudits(duplicateSsnEntities, "Duplicate SSNs found in the database.", "SSN", context);
            }
            else
            {
                // use case:  check if user is changing their social security number, there are no ssn duplicates
                foreach (var proposedChangeKv in demographicOracleHcmIdLookup.ToList())
                {
                    // lookup for existing demographic by oracleHcmId and check if ssn is different than proposed change
                    var demographicMarkedForSsnChange = existingEntities.FirstOrDefault(existingDemographic =>
                        existingDemographic.OracleHcmId == proposedChangeKv.Key && existingDemographic.Ssn != proposedChangeKv.Value.Ssn);

                    if (demographicMarkedForSsnChange != null)
                    {
                        // look for possible match with ssn for proposed change
                        var existingEmployeeMatchToProposedSsn = await context.Demographics
                            .FirstOrDefaultAsync(demographic => demographic.Ssn == proposedChangeKv.Value.Ssn, cancellationToken).ConfigureAwait(false);

                        if (existingEmployeeMatchToProposedSsn == null)
                        {
                            await AddDemographicSyncAuditAsync(demographicMarkedForSsnChange, "SSN changed with no conflicts.", "SSN", context, cancellationToken)
                                .ConfigureAwait(false);
                        }
                        // use case - ssn change for employee and ssn exists already in the system
                        else
                        {
                            // match for proposed ssn exists logic 
                            await HandleExistingEmployeeMatchToProposedSsnAsync(existingEmployeeMatchToProposedSsn, demographicMarkedForSsnChange, context, cancellationToken)
                                .ConfigureAwait(false);
                        }
                    }
                }
            }

            // checks for new ones and adds them as needed...
            await InsertNewDemographicsAsync(demographicsEntities, existingEntities, context, cancellationToken).ConfigureAwait(false);
            // checks for updates to existing demographics and updates them as needed...
            await UpdateExistingDemographicsAsync(demographicOracleHcmIdLookup, demographicSsnLookup, existingEntities, currentModificationDate, context, cancellationToken)
                .ConfigureAwait(false);

            // Save all changes to the database
            try
            {
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Failed to save batch: {DemographicsRequests}", employees);
            }

        }, cancellationToken);
    }

    /// <summary>
    /// Merges profit details from a source demographic to a target demographic.
    /// </summary>
    /// <remarks>This method retrieves all profit details associated with the source demographic, creates
    /// copies of them,  and associates the copies with the target demographic. An audit entry is created to log the
    /// operation. Changes are saved to the database, and any errors during the save operation are logged.</remarks>
    /// <param name="sourceDemographic">The demographic from which profit details will be copied.</param>
    /// <param name="targetDemographic">The demographic to which profit details will be added.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task MergeProfitDetailsToDemographic(Demographic sourceDemographic, Demographic targetDemographic, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseWritableContext(async context =>
        {
            var fakeSsn = await _fakeSsnService.GenerateFakeSsnAsync(cancellationToken).ConfigureAwait(false);
            sourceDemographic.Ssn = fakeSsn;

            await AddDemographicSyncAuditAsync(targetDemographic,
                $"Merging ProfitDetails from source OracleHcmId {sourceDemographic.OracleHcmId} to OracleHcmId {targetDemographic.OracleHcmId}", "ProfitDetails", context,
                cancellationToken).ConfigureAwait(false);

            // Save all changes to the database
            try
            {
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Failed to merge demographics: Source SSN {SourceSsn}, Target SSN {TargetSsn}", targetDemographic.Ssn.MaskSsn(),
                    targetDemographic.Ssn.MaskSsn());
            }

        }, cancellationToken);

    }

    /// <summary>
    /// Audits demographic synchronization errors by logging them into the database.
    /// </summary>
    /// <param name="badgeNumber">
    /// The badge number of the employee associated with the error.
    /// </param>
    /// <param name="oracleHcmId">
    /// The Oracle HCM identifier of the employee.
    /// </param>
    /// <param name="errorMessages">
    /// A collection of validation failures containing details about the errors.
    /// </param>
    /// <param name="requestedBy">
    /// The username of the individual who initiated the request.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <param name="args">
    /// Additional arguments providing context or metadata for the audit.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of auditing errors.
    /// </returns>
    /// <remarks>
    /// This method processes validation errors and records them in the <see cref="DemographicSyncAudit"/> table.
    /// It ensures that null values in the additional arguments are replaced with a default value.
    /// </remarks>
    public Task AuditError(int badgeNumber, long oracleHcmId, IEnumerable<ValidationFailure> errorMessages, string requestedBy, CancellationToken cancellationToken = default,
        params object?[] args)
    {
        return _dataContextFactory.UseWritableContext(c =>
        {
            for (int i = 0; i < args?.Length; i++)
            {
                args[i] ??= "null"; // Replace null with a default value
            }

            IEnumerable<DemographicSyncAudit> auditRecords = errorMessages.Select(e =>
                new DemographicSyncAudit
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

    #region Private Methods

    /// <summary>
    /// checks for updates to existing demographics and updates them as needed...
    /// </summary>
    /// <param name="demographicOracleHcmIdLookup"></param>
    /// <param name="demographicSsnLookup"></param>
    /// <param name="existingEntities"></param>
    /// <param name="currentModificationDate"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task UpdateExistingDemographicsAsync(Dictionary<long, Demographic> demographicOracleHcmIdLookup,
        ILookup<(int Ssn, int BadgeNumber), Demographic> demographicSsnLookup, List<Demographic> existingEntities, DateTimeOffset currentModificationDate,
        ProfitSharingDbContext context, CancellationToken cancellationToken)
    {
        // Update existing entities based on either OracleHcmId or SSN & BadgeNumber
        foreach (Demographic existingEntity in existingEntities)
        {
            Demographic? incomingEntity = null;

            // Prioritize matching by OracleHcmId, but fallback to SSN if OracleHcmId is missing (legacy case)
            if (demographicOracleHcmIdLookup.TryGetValue(existingEntity.OracleHcmId, out Demographic? entityByOracleHcmId))
            {
                incomingEntity = entityByOracleHcmId;
            }
            else
            {
                Demographic? entityBySsn = demographicSsnLookup[(existingEntity.Ssn, existingEntity.BadgeNumber)].FirstOrDefault();
                if (entityBySsn != null)
                {
                    incomingEntity = entityBySsn;
                }
            }

            // If we have a match, update the existing entity with new values
            if (incomingEntity != null)
            {
                // Correct OracleHcmId if it's missing or incorrect (for legacy records)
                // Assume all legacy records are below 2.1B and Oracle HCM ID is over that
                if (existingEntity.OracleHcmId != incomingEntity.OracleHcmId && existingEntity.OracleHcmId < int.MaxValue)
                {
                    existingEntity.OracleHcmId = incomingEntity.OracleHcmId;
                }

                await AddDemographicHistoryAsync(existingEntity, incomingEntity, currentModificationDate, context, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// inserts new demographics into the database.
    /// </summary>
    /// <param name="demographicsEntities"></param>
    /// <param name="existingEntities"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task InsertNewDemographicsAsync(List<Demographic> demographicsEntities, List<Demographic> existingEntities, ProfitSharingDbContext context,
        CancellationToken cancellationToken)
    {
        // Handle inserts for entities that do not exist in the database by OracleHcmId OR (SSN,BadgeNumber) pair
        HashSet<long> existingOracleHcmIds = existingEntities.Select(dbEntity => dbEntity.OracleHcmId).ToHashSet();
        HashSet<(int Ssn, int BadgeNumber)> existingSsnBadgePairs = existingEntities
            .Select(dbEntity => (dbEntity.Ssn, dbEntity.BadgeNumber))
            .ToHashSet();

        List<Demographic> newEntities = demographicsEntities
            .Where(entity => !existingOracleHcmIds.Contains(entity.OracleHcmId) &&
                             !existingSsnBadgePairs.Contains((entity.Ssn, entity.BadgeNumber)))
            .ToList();

        if (newEntities.Any())
        {
            // Bulk insert new entities
            foreach (Demographic entity in newEntities)
            {
                try
                {
                    context.Demographics.Add(entity);

                    DemographicHistory history = DemographicHistory.FromDemographic(entity);
                    context.DemographicHistories.Add(history);
                }
                catch (InvalidOperationException e) when (e.Message.Contains(
                                                              "When attaching existing entities, ensure that only one entity instance with a given key value is attached."))
                {
                    _logger.LogCritical(e, "Failed to process Demographic/OracleHCM employee record for BadgeNumber {BadgeNumber}", entity.BadgeNumber);
                    try
                    {
                        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch (CannotInsertNullException exception)
                    {
                        _logger.LogCritical(exception, "Failed to save Demographic/OracleHCM employee batch");
                    }
                    catch (OracleException exception)
                    {
                        _logger.LogCritical(exception, "Failed to save Demographic/OracleHCM employee batch");
                    }
                }
            }
        }
    }

    /// <summary>
    /// adds a demographic history record if there are changes to the demographic entity.
    /// </summary>
    /// <param name="existingEntity"></param>
    /// <param name="incomingEntity"></param>
    /// <param name="currentModificationDate"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task AddDemographicHistoryAsync(Demographic existingEntity, Demographic incomingEntity, DateTimeOffset currentModificationDate,
        ProfitSharingDbContext context, CancellationToken cancellationToken)
    {
        // Update the rest of the entity's fields
        bool updateHistory = !Demographic.DemographicHistoryEqual(existingEntity, incomingEntity);
        // if ssn change, will need to change any beneficiary and profitdetail records
        if (existingEntity.Ssn != incomingEntity.Ssn)
        {
            var beneficiariesToUpdate = context.BeneficiaryContacts.Where(b => b.Ssn == existingEntity.Ssn);
            await beneficiariesToUpdate.ForEachAsync(b => b.Ssn = incomingEntity.Ssn, cancellationToken).ConfigureAwait(false);
            var profitDetailsToUpdate = context.ProfitDetails.Where(p => p.Ssn == existingEntity.Ssn);
            await profitDetailsToUpdate.ForEachAsync(p => p.Ssn = incomingEntity.Ssn, cancellationToken).ConfigureAwait(false);
        }

        UpdateEntityValues(existingEntity, incomingEntity, currentModificationDate);

        if (updateHistory)
        {
            DemographicHistory newHistoryRecord = DemographicHistory.FromDemographic(incomingEntity, existingEntity.Id);
            DemographicHistory oldHistoryRecord = await context.DemographicHistories
                .Where(x => x.DemographicId == existingEntity.Id
                            && DateTimeOffset.UtcNow >= x.ValidFrom
                            && DateTimeOffset.UtcNow < x.ValidTo)
                .FirstAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            oldHistoryRecord.ValidTo = DateTimeOffset.UtcNow;
            newHistoryRecord.ValidFrom = oldHistoryRecord.ValidTo;
            context.DemographicHistories.Add(newHistoryRecord);
        }

    }

    /// <summary>
    /// handles the scenario where an existing employee's SSN matches a proposed SSN change.
    /// </summary>
    /// <param name="existingEmployeeMatchToProposedSsn"></param>
    /// existing employee in the system that matches the proposed ssn change
    /// <param name="demographicMarkedForSsnChange"></param>
    /// existing employee that is changing their ssn (different from existingEmployeeMatchToProposedSsn)
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task HandleExistingEmployeeMatchToProposedSsnAsync(Demographic existingEmployeeMatchToProposedSsn, Demographic demographicMarkedForSsnChange,
        ProfitSharingDbContext context, CancellationToken cancellationToken)
    {
        // check if customer has money...
        var result = await _totalService.GetVestingBalanceForSingleMemberAsync(SearchBy.Ssn, demographicMarkedForSsnChange.Ssn, (short)DateTime.Now.Year, cancellationToken)
            .ConfigureAwait(false);

        // check if terminated employee with no money and change 
        if (existingEmployeeMatchToProposedSsn.EmploymentStatusId == EmploymentStatus.Constants.Terminated && result?.CurrentBalance == (decimal)0.0)
        {
            await AddDemographicSyncAuditAsync(demographicMarkedForSsnChange, "Duplicate SSN found for terminated user with zero balance.", "SSN", context, cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            await AddDemographicSyncAuditAsync(demographicMarkedForSsnChange, "Duplicate SSN added for user.", "SSN", context, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// adds demographic sync audits for a list of duplicate SSN entities.
    /// </summary>
    /// <param name="duplicateSsnEntities"></param>
    /// <param name="message"></param>
    /// <param name="property"></param>
    /// <param name="context"></param>
    private static void AddDemographicSyncAudits(List<Demographic> duplicateSsnEntities, string message, string property, ProfitSharingDbContext context)
    {
        // Log duplicate SSN entries to the audit table
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

    /// <summary>
    /// adds a single demographic sync audit entry.
    /// </summary>
    /// <param name="demographic"></param>
    /// <param name="message"></param>
    /// <param name="property"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    private static async Task AddDemographicSyncAuditAsync(Demographic demographic, string message, string property, ProfitSharingDbContext context,
        CancellationToken cancellationToken)
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

    // Helper method to update entity fields
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

    /// <summary>
    /// uses inline sql to retrieve demographics from the database that match either OracleHcmId (primary) or (SSN + BadgeNumber) as a fallback.
    /// performance is much better than linq method - 35x (empirical prior measurement)
    /// have back up in case of failure to use existing linq method
    /// </summary>
    /// <param name="demographicsEntities"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<List<Demographic>> GetMatchingDemographicsWithSqlAsync(
        List<Demographic> demographicsEntities,
        ProfitSharingDbContext context,
        CancellationToken cancellationToken)
    {
        if (!demographicsEntities.Any())
        {
            return new List<Demographic>();
        }

        try
        {
            // Format Oracle HCM IDs as a comma-separated string (numeric - safe from injection)
            string hcmIdsInClause = string.Join(",", demographicsEntities.Select(d => d.OracleHcmId));

            // Build distinct (SSN,BadgeNumber) fallback pairs only for those where OracleHcmId might not be present/found.
            var ssnBadgePairs = demographicsEntities
                .Select(d => (d.Ssn, d.BadgeNumber))
                .Distinct()
                .ToList();

            // Guard: if all badge numbers are zero, log and skip fallback (avoid broad SSN-only scan intent)
            bool allZeroBadges = ssnBadgePairs.Count > 0 && ssnBadgePairs.All(p => p.BadgeNumber == 0);
            if (allZeroBadges)
            {
                _logger.LogCritical("All fallback demographic pairs have BadgeNumber == 0. Skipping (SSN,BadgeNumber) fallback lookup. OracleHcmCount={OracleHcmCount}", demographicsEntities.Count);
            }

            var ssnBadgeConditions = new List<string>();
            if (!allZeroBadges)
            {
                foreach (var pair in ssnBadgePairs)
                {
                    if (pair.BadgeNumber == 0)
                    {
                        continue; // skip invalid sentinel badge numbers
                    }
                    ssnBadgeConditions.Add($"(d.SSN = {pair.Ssn} AND d.BADGE_NUMBER = {pair.BadgeNumber})");
                }
            }

            string ssnBadgeWhereClause = ssnBadgeConditions.Count > 0 ? string.Join(" OR ", ssnBadgeConditions) : string.Empty;

            // Build SQL - if there are no fallback clauses we only query by OracleHcmId
            string sql;
            if (string.IsNullOrWhiteSpace(ssnBadgeWhereClause))
            {
                sql = $@"SELECT d.* FROM DEMOGRAPHIC d WHERE d.ORACLE_HCM_ID IN ({hcmIdsInClause})";
            }
            else
            {
                sql = $@"SELECT d.* FROM DEMOGRAPHIC d WHERE d.ORACLE_HCM_ID IN ({hcmIdsInClause}) OR ({ssnBadgeWhereClause})";
            }

            var result = await context.Demographics
                .FromSqlRaw(sql)
                .Include(d => d.ContactInfo)
                .Include(d => d.Address)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Retrieved {Count} matching demographics using direct SQL (OracleIds + Fallback SSN/Badge)", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing direct SQL query to find matching demographics. Falling back to LINQ method.");

            // Fallback to the standard method if direct SQL fails
            return await RetrieveDbChangedDemographicsAsync(demographicsEntities, context, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// method retrieves demographics from the database that match either OracleHcmId or SSN and BadgeNumber (fallback).
    /// </summary>
    /// <param name="demographicsEntities"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<List<Demographic>> RetrieveDbChangedDemographicsAsync(List<Demographic> demographicsEntities, ProfitSharingDbContext context,
        CancellationToken cancellationToken)
    {
        List<Demographic> existingEntities = new();

        if (demographicsEntities.Any())
        {
            var ssnBadgePairs = demographicsEntities.Select(d => (d.Ssn, d.BadgeNumber)).Distinct().ToList();
            var oracleHcmIds = demographicsEntities.Select(d => d.OracleHcmId).ToHashSet();

            // First, get all demographics with matching Oracle HCM IDs using an efficient Contains query
            if (oracleHcmIds.Any())
            {
                var entitiesByOracleHcmId = await context.Demographics
                    .Where(d => oracleHcmIds.Contains(d.OracleHcmId))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                existingEntities.AddRange(entitiesByOracleHcmId);
            }

            // Then, get all demographics with matching (SSN,BadgeNumber) pairs (excluding those already found by OracleHcmId)
            if (ssnBadgePairs.Any())
            {
                foreach (var pair in ssnBadgePairs)
                {
                    if (pair.BadgeNumber == 0)
                    {
                        continue; // skip invalid sentinel
                    }
                    var matches = await context.Demographics
                        .Where(d => d.Ssn == pair.Ssn && d.BadgeNumber == pair.BadgeNumber && !oracleHcmIds.Contains(d.OracleHcmId))
                        .ToListAsync(cancellationToken)
                        .ConfigureAwait(false);
                    existingEntities.AddRange(matches);
                }
            }

            // Remove duplicates (in case a record was found by both criteria)
            existingEntities = existingEntities.GroupBy(e => e.Id).Select(g => g.First()).ToList();
        }

        return existingEntities;
    }


    #endregion

}
