using System.Collections.Frozen;
using Demoulas.ProfitSharing.Common.ActivitySources;
using System.Diagnostics;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Mappers;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using System.Linq;

namespace Demoulas.ProfitSharing.Services;

public class DemographicsService : IDemographicsServiceInternal
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly DemographicMapper _mapper;

    public DemographicsService(IProfitSharingDataContextFactory dataContextFactory,
        DemographicMapper mapper)
    {
        _dataContextFactory = dataContextFactory;
        _mapper = mapper;
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
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(AddDemographicsStream), ActivityKind.Internal);
        var batch = new List<DemographicsRequest>();

        await foreach (var employee in employees.WithCancellation(cancellationToken))
        {
            batch.Add(employee);

            if (batch.Count >= batchSize)
            {
                _ = await UpsertDemographicsAsync(batch, cancellationToken);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            _ = await UpsertDemographicsAsync(batch, cancellationToken);
        }
    }

    public Task<DateTime?> GetLastOracleHcmSyncDate(CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(c =>
        {
            return c.Jobs.Where(c =>
                    c.JobStatusId == JobStatus.Constants.Completed && (c.JobTypeId == JobType.Constants.Full || c.JobTypeId == JobType.Constants.Delta))
                .MaxAsync(j => j.Completed, cancellationToken: cancellationToken);
        });
    }

    /// <summary>
    /// Inserts or updates a collection of demographic records asynchronously.
    /// </summary>
    /// <param name="demographicsRequests">The collection of demographic requests to be upserted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a set of demographic response DTOs.</returns>
    /// <remarks>
    /// This method performs an upsert operation, meaning it will insert new records and update existing ones based on the provided demographic requests.
    /// </remarks>
    private async Task<ISet<DemographicResponseDto>?> UpsertDemographicsAsync(IEnumerable<DemographicsRequest> demographicsRequests, CancellationToken cancellationToken)
    {
        DateTime currentModificationDate = DateTime.Now;

        // Use writable context for upsert operation
        List<Demographic> updatedEntities = await _dataContextFactory.UseWritableContext(async context =>
        {
            // Map incoming demographic requests to entity models
            List<Demographic> demographicsEntities = _mapper.Map(demographicsRequests).ToList();

            // Update LastModifiedDate for all entities
            demographicsEntities.ForEach(entity => entity.LastModifiedDate = currentModificationDate);

            // Create a lookup dictionary for fast access by OracleHcmId
            var demographicLookup = demographicsEntities.ToDictionary(entity => entity.OracleHcmId);

            // Fetch existing entities from the database that match the incoming OracleHcmIds
            var existingEntities = await context.Demographics
                                                .Where(dbEntity => demographicLookup.Keys.Contains(dbEntity.OracleHcmId))
                                                .ToListAsync(cancellationToken);

            // Extract existing OracleHcmIds from database entities
            var existingOracleHcmIds = existingEntities.Select(dbEntity => dbEntity.OracleHcmId).ToHashSet();

            // Determine which entities are new (i.e., need to be inserted)
            var newOracleHcmIds = demographicLookup.Keys.Except(existingOracleHcmIds).ToHashSet();

            if (newOracleHcmIds.Any())
            {
                // Bulk insert new entities
                var newEntities = demographicsEntities.Where(entity => newOracleHcmIds.Contains(entity.OracleHcmId)).ToList();
                context.Demographics.AddRange(newEntities);
            }

            // Update existing entities with new values
            foreach (var existingEntity in existingEntities)
            {
                if (demographicLookup.TryGetValue(existingEntity.OracleHcmId, out var incomingEntity))
                {
                    UpdateEntityValues(existingEntity, incomingEntity, currentModificationDate);
                }
            }

            // Save all changes
            await context.SaveChangesAsync(cancellationToken);
            return demographicsEntities;
        }, cancellationToken);

        // Map updated entities to response DTOs and return as a frozen set
        return _mapper.Map(updatedEntities).ToHashSet();
    }

    // Helper method to update entity fields
    private static void UpdateEntityValues(Demographic existingEntity, Demographic incomingEntity, DateTime modificationDate)
    {
        existingEntity.Ssn = incomingEntity.Ssn;
        existingEntity.BadgeNumber = incomingEntity.BadgeNumber;
        existingEntity.FullName = incomingEntity.FullName ?? $"{incomingEntity.LastName}, {incomingEntity.FirstName}";
        existingEntity.LastName = incomingEntity.LastName;
        existingEntity.FirstName = incomingEntity.FirstName;
        existingEntity.MiddleName = incomingEntity.MiddleName;
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
