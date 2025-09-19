using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Mappers;
using EntityFramework.Exceptions.Common;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using Exception = System.Exception;
using System.Linq.Expressions;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Provides services for handling demographic data related to employees within the Oracle HCM system.
/// </summary>
/// <remarks>
/// This service is responsible for processing, auditing, and managing demographic information.
/// It integrates with data context factories, mappers, and logging to ensure efficient and reliable operations.
/// </remarks>
internal class DemographicsService : IDemographicsServiceInternal
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly DemographicMapper _mapper;
    private readonly ILogger<DemographicsService> _logger;

    public DemographicsService(IProfitSharingDataContextFactory dataContextFactory,
        DemographicMapper mapper,
        ILogger<DemographicsService> logger)
    {
        _dataContextFactory = dataContextFactory;
        _mapper = mapper;
        _logger = logger;
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
        // Lookup keyed by (SSN, BadgeNumber) for fallback matching when OracleHcmId does not exist in DB
        ILookup<(int Ssn, int BadgeNumber), Demographic> demographicSsnBadgeLookup = demographicsEntities.ToLookup(entity => (entity.Ssn, entity.BadgeNumber));

        // Use writable context for the upsert operation
        return _dataContextFactory.UseWritableContext(async context =>
        {
            // 1. Fetch existing entities by OracleHcmId (primary key for matching)
            List<long> oracleIds = demographicOracleHcmIdLookup.Keys.ToList();
            List<Demographic> existingEntitiesByOracleId = await context.Demographics
                .Where(dbEntity => oracleIds.Contains(dbEntity.OracleHcmId))
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            HashSet<long> foundOracleIds = existingEntitiesByOracleId.Select(e => e.OracleHcmId).ToHashSet();

            // 2. Determine which incoming entities still need fallback lookup (no OracleHcmId match)
            var fallbackPairs = demographicsEntities
                .Where(e => !foundOracleIds.Contains(e.OracleHcmId))
                .Select(e => (e.Ssn, e.BadgeNumber))
                .Distinct()
                .ToList();

            List<Demographic> existingEntitiesBySsnBadge = new();
            if (fallbackPairs.Count > 0)
            {
                // Guard: If every fallback pair has BadgeNumber == 0 we consider this an invalid state
                // (likely upstream mapping/parsing error) and skip building the dynamic expression to avoid
                // an unbounded SSN-only lookup. Log at critical so it is surfaced.
                if (fallbackPairs.All(p => p.BadgeNumber == 0))
                {
                    _logger.LogCritical("All fallback demographic pairs have BadgeNumber == 0. Aborting (SSN,BadgeNumber) fallback lookup. OracleIdsProcessed={OracleIdCount} FallbackCount={FallbackCount}", foundOracleIds.Count, fallbackPairs.Count);
                }
                else
                {
                    // Build (SSN && BadgeNumber) OR expression to avoid cross-product false positives.
                    // (d.Ssn == p.Ssn && d.BadgeNumber == p.BadgeNumber) || ...
                    ParameterExpression param = Expression.Parameter(typeof(Demographic), "d");
                    Expression combined = Expression.Constant(false);

                    foreach (var pair in fallbackPairs)
                    {
                        if (pair.BadgeNumber == 0)
                        {
                            continue; // Skip invalid badge numbers inside mixed set
                        }
                        Expression ssnEq = Expression.Equal(
                            Expression.Property(param, nameof(Demographic.Ssn)),
                            Expression.Constant(pair.Ssn));
                        Expression badgeEq = Expression.Equal(
                            Expression.Property(param, nameof(Demographic.BadgeNumber)),
                            Expression.Constant(pair.BadgeNumber));
                        Expression and = Expression.AndAlso(ssnEq, badgeEq);
                        combined = Expression.OrElse(combined, and);
                    }

                    Expression<Func<Demographic, bool>> lambda = Expression.Lambda<Func<Demographic, bool>>(combined, param);
                    if (lambda.Body != Expression.Constant(false))
                    {
                        existingEntitiesBySsnBadge = await context.Demographics
                            .Where(lambda)
                            .ToListAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            // Merge results (distinct by Id to avoid duplicates if any record matched both criteria unexpectedly)
            Dictionary<int, Demographic> existingEntitiesMap = existingEntitiesByOracleId
                .Concat(existingEntitiesBySsnBadge)
                .GroupBy(e => e.Id)
                .ToDictionary(g => g.Key, g => g.First());

            List<Demographic> existingEntities = existingEntitiesMap.Values.ToList();

            // Handle potential duplicates in the existing database (SSN duplicates)
            List<Demographic> duplicateSsnEntities = existingEntities.GroupBy(e => e.Ssn)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .ToList();

            if (duplicateSsnEntities.Any())
            {
                // Log duplicate SSN entries to the audit table
                List<DemographicSyncAudit> audit = duplicateSsnEntities.Select(d => new DemographicSyncAudit
                {
                    BadgeNumber = d.BadgeNumber,
                    OracleHcmId = d.OracleHcmId,
                    InvalidValue = d.Ssn.MaskSsn(),
                    Message = "Duplicate SSNs found in the database.",
                    UserName = Constants.SystemAccountName,
                    PropertyName = "SSN"
                }).ToList();

                context.DemographicSyncAudit.AddRange(audit);
            }

            // Handle inserts for entities that do not exist in the database by OracleHcmId or SSN
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


            // Update existing entities based on either OracleHcmId or SSN & BadgeNumber
            foreach (Demographic existingEntity in existingEntities)
            {
                Demographic? incomingEntity = null;

                // Prioritize matching by OracleHcmId, but fallback to SSN if OracleHcmId is missing (legacy case)
                if (demographicOracleHcmIdLookup.TryGetValue(existingEntity.OracleHcmId, out Demographic? entityByOracleHcmId))
                {
                    incomingEntity = entityByOracleHcmId; // Primary match by OracleHcmId
                }
                else
                {
                    // Fallback precise match by (SSN, BadgeNumber)
                    Demographic? entityBySsnBadge = demographicSsnBadgeLookup[(existingEntity.Ssn, existingEntity.BadgeNumber)].FirstOrDefault();
                    if (entityBySsnBadge != null)
                    {
                        incomingEntity = entityBySsnBadge;
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

                    // Update the rest of the entity's fields
                    bool updateHistory = !Demographic.DemographicHistoryEqual(existingEntity, incomingEntity);

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
            }

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
            for (int i = 0; i < args.Length; i++)
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
}
