using System.Data.SqlTypes;
using System.Diagnostics;
using Bogus.Extensions.UnitedStates;
using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.OracleHcm;
using Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
using Demoulas.ProfitSharing.Services.InternalEntities;

namespace Demoulas.ProfitSharing.Services.Jobs;

public sealed class EmployeeSyncJob
{
    private readonly OracleDemographicsService _oracleDemographicsService;
    private readonly IDemographicsServiceInternal _demographicsService;
    private readonly OracleHcmConfig _oracleHcmConfig;

    public EmployeeSyncJob(OracleDemographicsService oracleDemographicsService,
        IDemographicsServiceInternal demographicsService,
        OracleHcmConfig oracleHcmConfig)
    {
        _oracleDemographicsService = oracleDemographicsService;
        _demographicsService = demographicsService;
        _oracleHcmConfig = oracleHcmConfig;
    }

    public async Task SynchronizeEmployees(CancellationToken cancellationToken)
    {
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(SynchronizeEmployees), ActivityKind.Internal);

        var lastSync = await _demographicsService.GetLastOracleHcmSyncDate(cancellationToken);

        var oracleHcmEmployees = _oracleDemographicsService.GetAllEmployees(cancellationToken);
        var requestDtoEnumerable = ConvertToRequestDto(oracleHcmEmployees);
        await _demographicsService.AddDemographicsStream(requestDtoEnumerable, _oracleHcmConfig.Limit, cancellationToken);
    }

    private async IAsyncEnumerable<DemographicsRequestDto> ConvertToRequestDto(IAsyncEnumerable<OracleEmployee?> asyncEnumerable)
    {
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(ConvertToRequestDto), ActivityKind.Internal);
        await foreach (OracleEmployee? employee in asyncEnumerable)
        {
            if (employee?.Address == null)
            {
                continue;
            }

            Bogus.Faker faker = new Bogus.Faker();
            yield return new DemographicsRequestDto
            {
                OracleHcmId = employee.PersonId,
                BadgeNumber = employee.BadgeNumber,
                DateOfBirth = employee.DateOfBirth,
                FirstName = employee.Name.FirstName,
                MiddleName = employee.Name.MiddleNames,
                LastName = employee.Name.LastName,
                FullName = employee.Name.DisplayName,
                HireDate = employee.WorkRelationship?.StartDate ?? SqlDateTime.MinValue.Value.ToDateOnly(),
                TerminationDate = employee.WorkRelationship?.TerminationDate,

                SSN = (employee.NationalIdentifier?.NationalIdentifierNumber ?? faker.Person.Ssn()).ConvertSsnToLong() ?? 0,
                StoreNumber = employee.WorkRelationship?.Assignment.LocationCode ?? 0,
                DepartmentId = employee.WorkRelationship?.Assignment.GetDepartmentId() ?? 0,
                PayClassificationId = employee.WorkRelationship?.Assignment.JobCode ?? 0,
                EmploymentTypeCode = faker.PickRandom('P', 'H', 'G', 'F'),
                PayFrequencyId = faker.PickRandom(PayFrequency.Constants.Weekly, PayFrequency.Constants.Monthly),
                EmploymentStatusId = employee.WorkRelationship?.TerminationDate == null ? EmploymentStatus.Constants.Active : EmploymentStatus.Constants.Terminated,
                GenderCode = faker.PickRandom('M', 'F', 'X'),



                ContactInfo = new ContactInfoRequestDto { PhoneNumber = employee.Phone?.PhoneNumber, EmailAddress = employee.Email?.EmailAddress },
                Address = new AddressRequestDto
                {
                    Street = employee.Address.AddressLine1,
                    Street2 = employee.Address.AddressLine2,
                    Street3 = employee.Address.AddressLine3,
                    Street4 = employee.Address.AddressLine4,
                    City = employee.Address.TownOrCity,
                    State = employee.Address.State,
                    PostalCode = employee.Address.PostalCode,
                    CountryISO = employee.Address.Country
                }
            };
        }
    }
}
