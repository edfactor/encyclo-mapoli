using System.Data.SqlTypes;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Extensions;
using Demoulas.ProfitSharing.OracleHcm.Validators;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Service responsible for synchronizing employee data from the Oracle HCM system to the Profit Sharing system.
/// This includes fetching employee data, validating it, and updating the Profit Sharing database.
/// </summary>
public sealed class EmployeeSyncService : IEmployeeSyncService
{
    private readonly OracleDemographicsSyncClient _oracleDemographicsSyncClient;
    private readonly IDemographicsServiceInternal _demographicsService;
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly OracleEmployeeValidator _employeeValidator;

    public EmployeeSyncService(HttpClient httpClient,
        IDemographicsServiceInternal demographicsService,
        IProfitSharingDataContextFactory profitSharingDataContextFactory,
        OracleHcmConfig oracleHcmConfig,
        OracleEmployeeValidator employeeValidator)
    {
        _oracleDemographicsSyncClient = new OracleDemographicsSyncClient(httpClient, oracleHcmConfig);
        _demographicsService = demographicsService;
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _oracleHcmConfig = oracleHcmConfig;
        _employeeValidator = employeeValidator;
    }

    public async Task SynchronizeEmployees(string requestedBy = "System", CancellationToken cancellationToken = default)
    {
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(SynchronizeEmployees), ActivityKind.Internal);

        var job = new Job
        {
            JobTypeId = JobType.Constants.EmployeeSyncFull,
            StartMethodId = StartMethod.Constants.System,
            RequestedBy = requestedBy,
            JobStatusId = JobStatus.Constants.Running,
            Started = DateTime.Now
        };

        await _profitSharingDataContextFactory.UseWritableContext(db =>
        {
            db.Jobs.Add(job);
            return db.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        bool success = true;
        try
        {
            await CleanAuditError(cancellationToken);
            var oracleHcmEmployees = _oracleDemographicsSyncClient.GetAllEmployees(cancellationToken);
            var requestDtoEnumerable = ConvertToRequestDto(oracleHcmEmployees, requestedBy, cancellationToken);
            await _demographicsService.AddDemographicsStream(requestDtoEnumerable, _oracleHcmConfig.Limit, cancellationToken);
        }
        catch (Exception ex)
        {
            success = false;
            await AuditError(0, new[] { new FluentValidation.Results.ValidationFailure("Error", ex.Message) }, requestedBy, cancellationToken);
        }
        finally
        {
            await _profitSharingDataContextFactory.UseWritableContext(db =>
            {
                return db.Jobs.Where(j=> j.Id == job.Id).ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.Completed, b => DateTime.Now)
                    .SetProperty(b => b.JobStatusId, b => success ? JobStatus.Constants.Completed : JobStatus.Constants.Failed),
                    cancellationToken: cancellationToken);
            }, cancellationToken);
        }
    }

    private Task AuditError(int badgeNumber, IEnumerable<FluentValidation.Results.ValidationFailure> errorMessages, string requestedBy, CancellationToken cancellationToken = default,
        params object?[] args)
    {
        return _profitSharingDataContextFactory.UseWritableContext(c =>
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
                    PropertyName = e.PropertyName
                });
            c.DemographicSyncAudit.AddRange(auditRecords);

            return c.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }

    private Task CleanAuditError(CancellationToken cancellationToken)
    {
        return _profitSharingDataContextFactory.UseWritableContext(c =>
        {
            DateTime clearBackTo = DateTime.Today.AddDays(-30);

            return c.DemographicSyncAudit.Where(t => t.Created < clearBackTo).ExecuteDeleteAsync(cancellationToken);
        }, cancellationToken);
    }

    private async IAsyncEnumerable<DemographicsRequest> ConvertToRequestDto(IAsyncEnumerable<OracleEmployee?> asyncEnumerable, 
        string requestedBy,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (OracleEmployee? employee in asyncEnumerable.WithCancellation(cancellationToken))
        {
            int badgeNumber = employee?.EmployeeId ?? 0;
            if (employee == null || badgeNumber == 0)
            {
                continue;
            }

            var result = await _employeeValidator.ValidateAsync(employee!, cancellationToken);
            if (!result.IsValid)
            {
                await AuditError(badgeNumber, result.Errors, requestedBy, cancellationToken);
                continue;
            }

            Bogus.Faker faker = new Bogus.Faker();
            yield return new DemographicsRequest
            {
                OracleHcmId = employee.PersonId,
                BadgeNumber = employee.EmployeeId,
                DateOfBirth = employee.DateOfBirth,
                HireDate = employee.WorkRelationship?.StartDate ?? SqlDateTime.MinValue.Value.ToDateOnly(),
                TerminationDate = employee.WorkRelationship?.TerminationDate,
                Ssn = (employee.NationalIdentifier?.NationalIdentifierNumber ?? faker.Person.Ssn()).ConvertSsnToLong() ?? 0,
                StoreNumber = employee.WorkRelationship?.Assignment.LocationCode ?? 0,
                DepartmentId = employee.WorkRelationship?.Assignment.GetDepartmentId() ?? 0,
                PayClassificationId = employee.WorkRelationship?.Assignment.JobCode ?? 0,
                EmploymentTypeCode = employee.WorkRelationship?.Assignment.GetEmploymentType() ?? char.MinValue,
                PayFrequencyId = employee.WorkRelationship?.Assignment.GetPayFrequency() ?? byte.MinValue,
                EmploymentStatusId =
                    employee.WorkRelationship?.TerminationDate == null ? EmploymentStatus.Constants.Active : EmploymentStatus.Constants.Terminated,
                GenderCode = employee.LegislativeInfoItem?.Gender switch
                {
                    "M" => Gender.Constants.Male,
                    "F" => Gender.Constants.Female,
                    "ORA_HRX_X" => Gender.Constants.Nonbinary,
                    _ => Gender.Constants.Unknown
                },
                ContactInfo = new ContactInfoRequestDto
                {
                    FirstName = employee.Name.FirstName,
                    MiddleName = employee.Name.MiddleNames,
                    LastName = employee.Name.LastName,
                    FullName = $"{employee.Name.LastName}, {employee.Name.FirstName}",
                    PhoneNumber = employee.Phone?.PhoneNumber,
                    EmailAddress = employee.Email?.EmailAddress
                },
                Address = new AddressRequestDto
                {
                    Street = employee.Address!.AddressLine1,
                    Street2 = employee.Address.AddressLine2,
                    Street3 = employee.Address.AddressLine3,
                    Street4 = employee.Address.AddressLine4,
                    City = employee.Address.TownOrCity,
                    State = employee.Address.State,
                    PostalCode = employee.Address.PostalCode,
                    CountryIso = employee.Address.Country
                }
            };
        }
    }
}
