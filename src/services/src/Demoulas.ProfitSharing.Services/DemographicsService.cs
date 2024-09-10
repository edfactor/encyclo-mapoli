using System.Collections.Frozen;
using Demoulas.ProfitSharing.Common.ActivitySources;
using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Demoulas.ProfitSharing.Common.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Services;

public class DemographicsService : IDemographicsServiceInternal
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly DemographicMapper _mapper;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DemographicsService> _logger;

    public DemographicsService(IProfitSharingDataContextFactory dataContextFactory,
        DemographicMapper mapper,
        IWebHostEnvironment environment,
        ILogger<DemographicsService> logger)
    {
        _dataContextFactory = dataContextFactory;
        _mapper = mapper;
        _environment = environment;
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
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(AddDemographicsStream), ActivityKind.Internal);
        await foreach (var employee in employees.WithCancellation(cancellationToken))
        {
            _ = await AddDemographic(employee, cancellationToken);
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
    /// Adds a collection of demographic records to the database.
    /// </summary>
    /// <param name="demographic">The collection of demographic requests to be added.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A set of demographic response DTOs if the operation is successful; otherwise, null.</returns>
    /// <exception cref="NotSupportedException">Thrown when the method is called in a production environment.</exception>
    /// <exception cref="DemographicException">Thrown when there is an error adding the demographic records to the database.</exception>
    /// <remarks>
    /// This method is not supported in a production environment.
    /// </remarks>
    private async Task<DemographicResponseDto> AddDemographic(DemographicsRequest demographic, CancellationToken cancellationToken)
    {
        if (_environment.IsProduction())
        {
            throw new NotSupportedException("This functionality is not supported in this environment.");
        }

        try
        {
            DateTime lastModificationDate = DateTime.Now;
            Demographic result = await _dataContextFactory.UseWritableContext(async context =>
            {

                Demographic entity = _mapper.Map(demographic);

#pragma warning disable S1854
                // need to load the object so we can save change back to the db.
                var db = await context.Demographics.FirstOrDefaultAsync(d => d.OracleHcmId == entity.OracleHcmId, cancellationToken: cancellationToken);
#pragma warning restore S1854

                if (db == null)
                {
                    db = context.Demographics.Add(entity).Entity;
                }
                else
                {
                    db.Ssn = entity.Ssn;
                    db.BadgeNumber = entity.BadgeNumber;
                    db.FullName = entity.FullName ?? $"{entity.LastName}, {entity.FirstName}";
                    db.LastName = entity.LastName;
                    db.FirstName = entity.FirstName;
                    db.MiddleName = entity.MiddleName;
                    db.StoreNumber = entity.StoreNumber;
                    db.DepartmentId = entity.DepartmentId;
                    db.PayClassificationId = entity.PayClassificationId;
                    db.ContactInfo = entity.ContactInfo;
                    db.Address = entity.Address;
                    db.DateOfBirth = entity.DateOfBirth;
                    db.FullTimeDate = entity.FullTimeDate;
                    db.HireDate = entity.HireDate;
                    db.ReHireDate = entity.ReHireDate;
                    db.TerminationCodeId = entity.TerminationCodeId;
                    db.TerminationDate = entity.TerminationDate;
                    db.EmploymentTypeId = entity.EmploymentTypeId;
                    db.PayFrequencyId = entity.PayFrequencyId;
                    db.GenderId = entity.GenderId;
                    db.EmploymentStatusId = entity.EmploymentStatusId;
                    db.LastModifiedDate = lastModificationDate;
                }


                await context.SaveChangesAsync(cancellationToken);
                return db;
            }, cancellationToken);

            return _mapper.Map(result)!;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to add to the {Demographic} table: {Message}.", nameof(Demographic), e.Message);
            throw new DemographicException("Unable to add new Demographic object");
        }
    }
}
