using System.Data.SqlTypes;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Bogus.Extensions.UnitedStates;
using Demoulas.Common.Caching.Interfaces;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm;
using Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.ProfitSharing.Services.InternalEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Jobs;

public sealed class EmployeeSyncJob : IEmployeeSyncJob
{
    private const int MAX_STORE_ID = 899;
    private readonly OracleDemographicsService _oracleDemographicsService;
    private readonly IDemographicsServiceInternal _demographicsService;
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly IBaseCacheService<PayClassificationResponseCache> _payCacheService;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly ILogger<EmployeeSyncJob> _logger;

    public EmployeeSyncJob(OracleDemographicsService oracleDemographicsService,
        IDemographicsServiceInternal demographicsService,
        IProfitSharingDataContextFactory profitSharingDataContextFactory,
        IBaseCacheService<PayClassificationResponseCache> payCacheService,
        OracleHcmConfig oracleHcmConfig,
        ILogger<EmployeeSyncJob> logger)
    {
        _oracleDemographicsService = oracleDemographicsService;
        _demographicsService = demographicsService;
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _payCacheService = payCacheService;
        _oracleHcmConfig = oracleHcmConfig;
        _logger = logger;
    }

    public async Task<bool> SynchronizeEmployee(int badgeNumber, CancellationToken cancellationToken)
    {
        try
        {
            using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(SynchronizeEmployee), ActivityKind.Internal);
            var employee = await _demographicsService.GetDemographicByBadgeNumber(badgeNumber, cancellationToken);

            if (employee == null)
            {
                _logger.LogError("Unable to find employee with badge number of '{badgeNumber}'", badgeNumber);
                return false;
            }

            async IAsyncEnumerable<OracleEmployee?> GetSingleValueAsync(long oracleHcmId)
            {
                // Yield return a single value
                yield return await _oracleDemographicsService.GetEmployee(oracleHcmId, cancellationToken);
            }


            var payClassifications = await GetPayClassifications(cancellationToken);
            var departments = await GetDepartments(cancellationToken);
            var requestDtoEnumerable = ConvertToRequestDto(GetSingleValueAsync(employee.OracleHcmId), payClassifications, departments, cancellationToken);
            var user = requestDtoEnumerable.ToBlockingEnumerable(cancellationToken).ToHashSet();
            if (!user.Any())
            {
                return false;
            }

            var response = await _demographicsService.AddDemographics(user, cancellationToken);
            return response != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to Synchronize Employee {badgeNumber} because {message}.", badgeNumber, ex.Message);
            return false;
        }
    }

    public async Task SynchronizeEmployees(CancellationToken cancellationToken)
    {
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(SynchronizeEmployees), ActivityKind.Internal);

        var lastSync = await _demographicsService.GetLastOracleHcmSyncDate(cancellationToken);
        var payClassifications = await GetPayClassifications(cancellationToken);
        var departments = await GetDepartments(cancellationToken);

        var oracleHcmEmployees = _oracleDemographicsService.GetAllEmployees(cancellationToken);
        var requestDtoEnumerable = ConvertToRequestDto(oracleHcmEmployees, payClassifications, departments, cancellationToken);
        await _demographicsService.AddDemographicsStream(requestDtoEnumerable, _oracleHcmConfig.Limit, cancellationToken);
    }

    private Task AuditError(int badgeNumber, string messageTemplate, IAppUser? appUser = null, CancellationToken cancellationToken = default, params object?[] args)
    {
        return _profitSharingDataContextFactory.UseWritableContext(c =>
        {
            for (int i = 0; i < args.Length; i++)
            {
                args[i] ??= "null"; // Replace null with a default value
            }

            DemographicSyncAudit audit = new DemographicSyncAudit
            {
                BadgeNumber = badgeNumber,
                Message = messageTemplate.ReplaceNamedParams(args),
                UserName = appUser?.UserName
            };
            c.DemographicSyncAudit.AddAsync(audit);
            return c.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }



    private async Task<ISet<byte>> GetPayClassifications(CancellationToken cancellationToken)
    {
        var payClassificationResponseCaches = await _payCacheService.GetAllAsync(cancellationToken);
        return payClassificationResponseCaches.Select(p => p.Id).ToHashSet();
    }

    private Task<List<byte>> GetDepartments(CancellationToken cancellationToken)
    {
        return _profitSharingDataContextFactory.UseReadOnlyContext(c =>
        {
            return c.Departments.Select(d => d.Id).ToListAsync(cancellationToken);
        });
    }

    private async IAsyncEnumerable<DemographicsRequestDto> ConvertToRequestDto(IAsyncEnumerable<OracleEmployee?> asyncEnumerable, 
        ISet<byte> payClassifications, 
        IList<byte> departments,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(ConvertToRequestDto), ActivityKind.Internal);
        await foreach (OracleEmployee? employee in asyncEnumerable.WithCancellation(cancellationToken))
        {
            int badgeNumber = employee?.BadgeNumber ?? 0;
            if (employee?.Address == null)
            {
                string messageTemplate = "No address found for employee with '{BadgeNumber}'.";
                _logger.LogCritical(messageTemplate, employee?.BadgeNumber);
                await AuditError(badgeNumber, messageTemplate, args: badgeNumber, cancellationToken: cancellationToken);
                continue;
            }

            if (employee.WorkRelationship?.Assignment.GetEmploymentType() is null or char.MinValue)
            {
                string messageTemplate = "Unable to determine Employment Type for employee with BadgeNumber {BadgeNumber}. Value received is '{FullPartTime}' ";
                _logger.LogCritical(messageTemplate, badgeNumber, employee.WorkRelationship?.Assignment.FullPartTime ?? "null");
                await AuditError(badgeNumber, messageTemplate, args: new object[] { badgeNumber, employee.WorkRelationship?.Assignment.FullPartTime ?? "null" },
                    cancellationToken: cancellationToken);
                continue;
            }

            if (!payClassifications.Contains(employee.WorkRelationship?.Assignment.JobCode ?? 0))
            {
                string messageTemplate = "Unknown pay classification for employee with BadgeNumber {BadgeNumber}. Value received is '{JobCode}' ";
                _logger.LogCritical(messageTemplate, badgeNumber, employee.WorkRelationship?.Assignment.JobCode);
                await AuditError(badgeNumber, messageTemplate, appUser: null, cancellationToken: cancellationToken, badgeNumber, employee.WorkRelationship?.Assignment.JobCode);
                continue;
            }

            if (!departments.Contains(employee.WorkRelationship?.Assignment.GetDepartmentId() ?? 0))
            {
                string messageTemplate = "Unknown department for employee with BadgeNumber {BadgeNumber}. Value received is '{PositionCode}' ";
                _logger.LogCritical(messageTemplate, badgeNumber, employee.WorkRelationship?.Assignment.PositionCode);
                await AuditError(badgeNumber, messageTemplate, appUser: null, cancellationToken: cancellationToken, badgeNumber,
                    employee.WorkRelationship?.Assignment.PositionCode);
                continue;
            }

            if (employee.WorkRelationship?.Assignment.GetPayFrequency() is null or byte.MinValue)
            {
                string messageTemplate = "Unknown pay frequency for employee with BadgeNumber {BadgeNumber}. Value received is '{Frequency}'";
                _logger.LogCritical(messageTemplate, badgeNumber, employee.WorkRelationship?.Assignment.Frequency);
                await AuditError(badgeNumber, messageTemplate, appUser: null, cancellationToken: cancellationToken, badgeNumber, employee.WorkRelationship?.Assignment.Frequency);
                continue;
            }

            if (employee.WorkRelationship?.Assignment.GetPayFrequency() is null or byte.MinValue)
            {
                _logger.LogCritical("Unknown pay frequency for employee with BadgeNumber {BadgeNumber}. Value received is '{Frequency}' ", employee.BadgeNumber,
                    employee.WorkRelationship?.Assignment.Frequency);
                continue;
            }

            if (employee.WorkRelationship?.Assignment.LocationCode > MAX_STORE_ID)
            {
                _logger.LogCritical("Unknown store location for employee with BadgeNumber{BadgeNumber}. Value received is '{LocationCode}' ", employee.BadgeNumber,
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
