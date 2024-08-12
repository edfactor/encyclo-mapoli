using System.Data.SqlTypes;
using System.Diagnostics;
using System.Threading;
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
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Jobs;

public sealed class EmployeeSyncJob : IEmployeeSyncJob
{
    private const int MAX_STORE_ID = 899;
    private readonly OracleDemographicsService _oracleDemographicsService;
    private readonly IDemographicsServiceInternal _demographicsService;
    private readonly IBaseCacheService<PayClassificationResponseCache> _payCacheService;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly ILogger<EmployeeSyncJob> _logger;

    public EmployeeSyncJob(OracleDemographicsService oracleDemographicsService,
        IDemographicsServiceInternal demographicsService,
        IBaseCacheService<PayClassificationResponseCache> payCacheService,
        OracleHcmConfig oracleHcmConfig,
        ILogger<EmployeeSyncJob> logger)
    {
        _oracleDemographicsService = oracleDemographicsService;
        _demographicsService = demographicsService;
        _payCacheService = payCacheService;
        _oracleHcmConfig = oracleHcmConfig;
        _logger = logger;
    }

    public async Task SynchronizeEmployee(int badgeNumber, CancellationToken cancellationToken)
    {
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(SynchronizeEmployee), ActivityKind.Internal);
        var employee = await _demographicsService.GetDemographicByBadgeNumber(badgeNumber, cancellationToken);

        if (employee == null)
        {
            _logger.LogError("Unable to find employee with badge number of '{badgeNumber}'", badgeNumber);
            return;
        }

        var lastSync = await _demographicsService.GetLastOracleHcmSyncDate(cancellationToken);

        var payClassifications = await GetPayClassifications(cancellationToken);
        var oracleHcmEmployees = _oracleDemographicsService.GetAllEmployees(cancellationToken);
        var requestDtoEnumerable = ConvertToRequestDto(oracleHcmEmployees, payClassifications);
        await _demographicsService.AddDemographicsStream(requestDtoEnumerable, _oracleHcmConfig.Limit, cancellationToken);
    }

    public async Task SynchronizeEmployees(CancellationToken cancellationToken)
    {
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(SynchronizeEmployees), ActivityKind.Internal);

        var lastSync = await _demographicsService.GetLastOracleHcmSyncDate(cancellationToken);
        var payClassifications = await GetPayClassifications(cancellationToken);

        var oracleHcmEmployees = _oracleDemographicsService.GetAllEmployees(cancellationToken);
        var requestDtoEnumerable = ConvertToRequestDto(oracleHcmEmployees, payClassifications);
        await _demographicsService.AddDemographicsStream(requestDtoEnumerable, _oracleHcmConfig.Limit, cancellationToken);
    }

    private async Task<ISet<byte>> GetPayClassifications(CancellationToken cancellationToken)
    {
        var payClassificationResponseCaches = await _payCacheService.GetAllAsync(cancellationToken);
        return payClassificationResponseCaches.Select(p => p.Id).ToHashSet();
    }

    private async IAsyncEnumerable<DemographicsRequestDto> ConvertToRequestDto(IAsyncEnumerable<OracleEmployee?> asyncEnumerable, ISet<byte> payClassifications)
    {
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(ConvertToRequestDto), ActivityKind.Internal);
        await foreach (OracleEmployee? employee in asyncEnumerable)
        {
            if (employee?.Address == null)
            {
                _logger.LogCritical("No address found for employee with '{BadgeNumber}'.", employee?.BadgeNumber);
                continue;
            }

            if (employee.WorkRelationship?.Assignment.GetEmploymentType() is null or char.MinValue)
            {
                _logger.LogCritical("Unable to determine Employment Type for employee with {BadgeNumber}. Value received is '{FullPartTime}' ",
                    employee.BadgeNumber, employee.WorkRelationship?.Assignment.FullPartTime ?? "null");
                continue;
            }

            if (!payClassifications.Contains(employee.WorkRelationship?.Assignment.JobCode ?? 0))
            {
                _logger.LogCritical("Unknown pay classification for employee with {BadgeNumber}. Value received is '{JobCode}' ", employee.BadgeNumber,
                    employee.WorkRelationship?.Assignment.JobCode);
                continue;
            }

            if (employee.WorkRelationship?.Assignment.GetPayFrequency() is null or byte.MinValue)
            {
                _logger.LogCritical("Unknown pay frequency for employee with {BadgeNumber}. Value received is '{Frequency}' ", employee.BadgeNumber,
                    employee.WorkRelationship?.Assignment.Frequency);
                continue;
            }

            if (employee.WorkRelationship?.Assignment.GetPayFrequency() is null or byte.MinValue)
            {
                _logger.LogCritical("Unknown pay frequency for employee with {BadgeNumber}. Value received is '{Frequency}' ", employee.BadgeNumber,
                    employee.WorkRelationship?.Assignment.Frequency);
                continue;
            }

            if (employee.WorkRelationship?.Assignment.LocationCode > MAX_STORE_ID)
            {
                _logger.LogCritical("Unknown store location for employee with {BadgeNumber}. Value received is '{LocationCode}' ", employee.BadgeNumber,
                    employee.WorkRelationship?.Assignment.LocationCode);
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
                EmploymentTypeCode = employee.WorkRelationship?.Assignment.GetEmploymentType() ?? char.MinValue,
                PayFrequencyId = employee.WorkRelationship?.Assignment.GetPayFrequency() ?? byte.MinValue,
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
