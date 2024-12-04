using Demoulas.ProfitSharing.Common.ActivitySources;
using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Mappers;
using Microsoft.EntityFrameworkCore;
using Demoulas.ProfitSharing.Data.Extensions;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using EntityFramework.Exceptions.Common;

namespace Demoulas.ProfitSharing.Services;

public class DemographicsService : IDemographicsServiceInternal
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
    /// Asynchronously processes a stream of demographic data for employees, adding them in batches.
    /// </summary>
    /// <param name="employees">An asynchronous enumerable of <see cref="DemographicsRequest"/> representing the employees' demographic data.</param>
    /// <param name="batchSize">The size of each batch to be processed. Defaults to <see cref="byte.MaxValue"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// This method processes the demographic data in batches to optimize performance and resource usage.
    /// </remarks>
    public async Task AddDemographicsStream(IAsyncEnumerable<DemographicsRequest> employees, byte batchSize = byte.MaxValue,
        CancellationToken cancellationToken = default)
    {
        var batch = new List<DemographicsRequest>();

        await foreach (var employee in employees.WithCancellation(cancellationToken))
        {
            batch.Add(employee);

            if (batch.Count >= batchSize)
            {
                await UpsertDemographicsAsync(batch, cancellationToken);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            await UpsertDemographicsAsync(batch, cancellationToken);
        }
    }

    public Task<DateTime?> GetLastOracleHcmSyncDate(CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(c =>
        {
            return c.Jobs.Where(c =>
                    c.JobStatusId == JobStatus.Constants.Completed && (c.JobTypeId == JobType.Constants.EmployeeSyncFull || c.JobTypeId == JobType.Constants.PayrollSyncFull))
                .MaxAsync(j => j.Completed, cancellationToken: cancellationToken);
        });
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

        // Use writable context for the upsert operation
        return _dataContextFactory.UseWritableContext(async context =>
        {
            // Map incoming demographic requests to entity models
            List<Demographic> demographicsEntities = _mapper.Map(demographicsRequests).ToList();

            // Update LastModifiedDate for all entities
            demographicsEntities.ForEach(entity => entity.LastModifiedDate = currentModificationDate);

            // Create lookup dictionaries for both OracleHcmId and SSN
            var demographicOracleHcmIdLookup = demographicsEntities.ToDictionary(entity => entity.OracleHcmId);
            var demographicSsnLookup = demographicsEntities.ToLookup(entity => (entity.Ssn, entity.EmployeeId));
            var ssnCollection = demographicsEntities.Select(d => d.Ssn).ToHashSet();
            var dobCollection = demographicsEntities.Select(d => d.DateOfBirth).ToHashSet();

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
                    BadgeNumber = d.EmployeeId,
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
                    }
                    catch (InvalidOperationException e) when (e.Message.Contains(
                                                                  "When attaching existing entities, ensure that only one entity instance with a given key value is attached."))
                    {
                        _logger.LogCritical(e, "Failed to process Demographic/OracleHCM employee record for EmployeeId {EmployeeId}", entity.EmployeeId);
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


            // Update existing entities based on either OracleHcmId or SSN & EmployeeId
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
                    var entityBySsn = demographicSsnLookup[(existingEntity.Ssn, existingEntity.EmployeeId)].FirstOrDefault();
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
                    UpdateEntityValues(existingEntity, incomingEntity, currentModificationDate);
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
        existingEntity.EmployeeId = incomingEntity.EmployeeId;
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
}
