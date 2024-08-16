using System.Data.SqlTypes;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Bogus.Extensions.UnitedStates;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Validators;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

public sealed class EmployeeSyncService : IEmployeeSyncService
{
    private readonly OracleDemographicsService _oracleDemographicsService;
    private readonly IDemographicsServiceInternal _demographicsService;
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly OracleEmployeeValidator _employeeValidator;

    public EmployeeSyncService(OracleDemographicsService oracleDemographicsService,
        IDemographicsServiceInternal demographicsService,
        IProfitSharingDataContextFactory profitSharingDataContextFactory,
        OracleHcmConfig oracleHcmConfig,
        OracleEmployeeValidator employeeValidator)
    {
        _oracleDemographicsService = oracleDemographicsService;
        _demographicsService = demographicsService;
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _oracleHcmConfig = oracleHcmConfig;
        _employeeValidator = employeeValidator;
    }

    public async Task SynchronizeEmployees(CancellationToken cancellationToken)
    {
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(SynchronizeEmployees), ActivityKind.Internal);

        var oracleHcmEmployees = _oracleDemographicsService.GetAllEmployees(cancellationToken);
        var requestDtoEnumerable = ConvertToRequestDto(oracleHcmEmployees, cancellationToken);
        await _demographicsService.AddDemographicsStream(requestDtoEnumerable, _oracleHcmConfig.Limit, cancellationToken);
    }

    private Task AuditError(int badgeNumber, IEnumerable<FluentValidation.Results.ValidationFailure> errorMessages, IAppUser? appUser = null, CancellationToken cancellationToken = default,
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
                    UserName = appUser?.UserName ?? "System",
                    PropertyName = e.PropertyName
                });
            c.DemographicSyncAudit.AddRange(auditRecords);

            return c.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }

    private async IAsyncEnumerable<DemographicsRequestDto> ConvertToRequestDto(IAsyncEnumerable<OracleEmployee?> asyncEnumerable,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var activity = OracleHcmActivitySource.Instance.StartActivity(nameof(ConvertToRequestDto), ActivityKind.Internal);
        await foreach (OracleEmployee? employee in asyncEnumerable.WithCancellation(cancellationToken))
        {
            int badgeNumber = employee?.BadgeNumber ?? 0;
            if (employee == null || badgeNumber == 0)
            {
                continue;
            }

            var result = await _employeeValidator.ValidateAsync(employee!, cancellationToken);
            if (!result.IsValid)
            {
                await AuditError(badgeNumber, result.Errors, null, cancellationToken);
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
                    Street = employee.Address!.AddressLine1,
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
