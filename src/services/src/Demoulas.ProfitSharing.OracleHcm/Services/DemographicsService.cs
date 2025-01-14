using System.Collections.Concurrent;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Factories;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Mappers;
using EntityFramework.Exceptions.Common;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

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
    private readonly ConcurrentQueue<DemographicsRequest> _requests;

    public DemographicsService(IProfitSharingDataContextFactory dataContextFactory,
        DemographicMapper mapper,
        ILogger<DemographicsService> logger)
    {
        _dataContextFactory = dataContextFactory;
        _mapper = mapper;
        _logger = logger;
        _requests = new ConcurrentQueue<DemographicsRequest>();
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
   public async Task AddDemographicsStreamAsync(IAsyncEnumerable<DemographicsRequest> employees, byte batchSize = byte.MaxValue,
       CancellationToken cancellationToken = default)
   {
       const int throttleLimit = 10_000; // Max queue size for safety (configurable)
       var batch = new Dictionary<long, DemographicsRequest>();
       bool batchProcessed = false;
       await foreach (var employee in employees.WithCancellation(cancellationToken))
       {
           // Throttle queue size
           while (_requests.Count >= throttleLimit)
           {
               await Task.Delay(50, cancellationToken); // Wait to prevent unbounded growth
           }

           _requests.Enqueue(employee);

           // Process batch when batchSize is reached during enqueue
           if (_requests.Count >= batchSize)
           {
               while (_requests.TryDequeue(out var demoRequest))
               {
                   if (!batch.TryAdd(demoRequest.OracleHcmId, demoRequest))
                   {
                       _logger.LogError("Duplicate OracleHcmId: {OracleHcmId} found; skipping....", demoRequest.OracleHcmId);
                   }

                   if (batch.Count == batchSize)
                   {
                       break;
                   }
               }

               if (batch.Count > 0)
               {
                   batchProcessed = await ProcessBatch();
               }
           }
       }

       // Process any leftover requests in the batch
       if (batch.Count > 0 && batchProcessed)
       {
           _ = await ProcessBatch();
       }

       async Task<bool> ProcessBatch()
       {
           await UpsertDemographicsAsync(batch.Values, cancellationToken);
           batchProcessed = true;
           batch.Clear(); // Clear batch after processing
           return batchProcessed;
       }
   }

    /// <summary>
    /// Asynchronously inserts or updates a collection of demographic records in the database.
    /// </summary>
    /// <param name="demographicsRequests">A collection of <see cref="DemographicsRequest"/> objects representing the demographic data to be upserted.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete. This token can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous upsert operation.</returns>
    /// <remarks>
    /// This method ensures that demographic records are either inserted as new entries or updated if they already exist in the database.
    /// The operation is performed within a writable database context to maintain data integrity.
    /// </remarks>
    private Task UpsertDemographicsAsync(IEnumerable<DemographicsRequest> demographicsRequests, CancellationToken cancellationToken)
    {
        DateTime currentModificationDate = DateTime.Now;

        // Map incoming demographic requests to entity models
        List<Demographic> demographicsEntities = _mapper.Map(demographicsRequests).ToList();

        // Update LastModifiedDate for all entities
        demographicsEntities.ForEach(entity => entity.LastModifiedDate = currentModificationDate);

        // Create lookup dictionaries for both OracleHcmId and SSN
        var demographicOracleHcmIdLookup = demographicsEntities.ToDictionary(entity => entity.OracleHcmId);
        var demographicSsnLookup = demographicsEntities.ToLookup(entity => (entity.Ssn, BadgeNumber: entity.BadgeNumber));
        var ssnCollection = demographicsEntities.Select(d => d.Ssn).ToHashSet();
        var dobCollection = demographicsEntities.Select(d => d.DateOfBirth).ToHashSet();

        // Use writable context for the upsert operation
        return _dataContextFactory.UseWritableContext(async context =>
        {
            // Fetch existing entities from the database using both OracleHcmId and SSN
            var existingEntities = await context.Demographics
                .Where(dbEntity => demographicOracleHcmIdLookup.Keys.Contains(dbEntity.OracleHcmId) ||
                                   (ssnCollection.Contains(dbEntity.Ssn) && dobCollection.Contains(dbEntity.DateOfBirth)))
                .ToListAsync(cancellationToken);

            // Handle potential duplicates in the existing database (SSN duplicates)
            var duplicateSsnEntities = existingEntities.GroupBy(e => e.Ssn)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .ToList();

            if (duplicateSsnEntities.Any())
            {
                // Log duplicate SSN entries to the audit table
                var audit = duplicateSsnEntities.Select(d => new DemographicSyncAudit
                {
                    BadgeNumber = d.BadgeNumber,
                    InvalidValue = d.Ssn.MaskSsn(),
                    Message = "Duplicate SSNs found in the database.",
                    UserName = "System",
                    PropertyName = "SSN"
                }).ToList();

                context.DemographicSyncAudit.AddRange(audit);
            }

            // Handle inserts for entities that do not exist in the database by OracleHcmId or SSN
            var existingOracleHcmIds = existingEntities.Select(dbEntity => dbEntity.OracleHcmId).ToHashSet();
            var existingSsns = existingEntities.Select(dbEntity => dbEntity.Ssn).ToHashSet();

            var newEntities = demographicsEntities
                .Where(entity => !existingOracleHcmIds.Contains(entity.OracleHcmId) && !existingSsns.Contains(entity.Ssn))
                .ToList();

            if (newEntities.Any())
            {
                // Bulk insert new entities

                foreach (var entity in newEntities)
                {
                    try
                    {
                        context.Demographics.Add(entity);

                        var history = DemographicHistory.FromDemographic(entity);
                        context.DemographicHistories.Add(history);
                    }
                    catch (InvalidOperationException e) when (e.Message.Contains(
                                                                  "When attaching existing entities, ensure that only one entity instance with a given key value is attached."))
                    {
                        _logger.LogCritical(e, "Failed to process Demographic/OracleHCM employee record for BadgeNumber {BadgeNumber}", entity.BadgeNumber);
                        try
                        {
                            await context.SaveChangesAsync(cancellationToken);
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
            foreach (var existingEntity in existingEntities)
            {
                Demographic? incomingEntity = null;

                // Prioritize matching by OracleHcmId, but fallback to SSN if OracleHcmId is missing (legacy case)
                if (demographicOracleHcmIdLookup.TryGetValue(existingEntity.OracleHcmId, out var entityByOracleHcmId))
                {
                    incomingEntity = entityByOracleHcmId;
                }
                else
                {
                    var entityBySsn = demographicSsnLookup[(existingEntity.Ssn, existingEntity.BadgeNumber)].FirstOrDefault();
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

                    // Update the rest of the entity's fields
                    bool updateHistory = !Demographic.DemographicHistoryEqual(existingEntity, incomingEntity);

                    UpdateEntityValues(existingEntity, incomingEntity, currentModificationDate);
                    if (updateHistory)
                    {
                        var newHistoryRecord = DemographicHistory.FromDemographic(incomingEntity, existingEntity.Id);
                        var oldHistoryRecord = await context.DemographicHistories
                            .Where(x => x.DemographicId == existingEntity.Id && DateTime.UtcNow >= x.ValidFrom && DateTime.UtcNow < x.ValidTo)
                            .FirstAsync(cancellationToken: cancellationToken);
                        oldHistoryRecord.ValidTo = DateTime.UtcNow;
                        newHistoryRecord.ValidFrom = oldHistoryRecord.ValidTo;
                        context.DemographicHistories.Add(newHistoryRecord);
                    }
                }
            }

            // Save all changes to the database
            await context.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }

    // Helper method to update entity fields
    private static void UpdateEntityValues(Demographic existingEntity, Demographic incomingEntity, DateTime modificationDate)
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
        existingEntity.LastModifiedDate = modificationDate;
    }

    public Task AuditError(int badgeNumber, IEnumerable<ValidationFailure> errorMessages, string requestedBy, CancellationToken cancellationToken = default,
        params object?[] args)
    {
        return _dataContextFactory.UseWritableContext(c =>
        {
            for (int i = 0; i < args.Length; i++)
            {
                args[i] ??= "null"; // Replace null with a default value
            }

            var auditRecords = errorMessages.Select(e =>
                new DemographicSyncAudit
                {
                    BadgeNumber = badgeNumber,
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
