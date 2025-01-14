using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using Bogus;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Extensions;
using Demoulas.ProfitSharing.OracleHcm.Validators;
using Demoulas.Util.Extensions;
using MassTransit;

namespace Demoulas.ProfitSharing.OracleHcm.Messaging;
internal class EmployeeSyncConsumer : IConsumer<MessageRequest<OracleEmployee>>
{
    private readonly OracleEmployeeValidator _employeeValidator;
    private readonly IDemographicsServiceInternal _demographicsService;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly Faker _faker = new Faker();

    public EmployeeSyncConsumer(OracleEmployeeValidator employeeValidator,
        IDemographicsServiceInternal demographicsServiceInternal,
        OracleHcmConfig oracleHcmConfig)
    {
        _employeeValidator = employeeValidator;
        _demographicsService = demographicsServiceInternal;
        _oracleHcmConfig = oracleHcmConfig;
    }

    public Task Consume(ConsumeContext<MessageRequest<OracleEmployee>> context)
    {
        var employee = context.Message.Body;
        var requestDtoEnumerable = ConvertToRequestDto(employee, context.Message.UserId, context.CancellationToken);
        return _demographicsService.AddDemographicsStreamAsync(requestDtoEnumerable, _oracleHcmConfig.Limit, context.CancellationToken);
    }
    private async IAsyncEnumerable<DemographicsRequest> ConvertToRequestDto(OracleEmployee employee,
        string requestedBy,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {

        int badgeNumber = employee?.BadgeNumber ?? 0;
        if (employee == null || badgeNumber == 0)
        {
            yield break;
        }

        var result = await _employeeValidator.ValidateAsync(employee!, cancellationToken);
        if (!result.IsValid)
        {
            await _demographicsService.AuditError(badgeNumber, result.Errors, requestedBy, cancellationToken);
            yield break;
        }

        yield return new DemographicsRequest
        {
            OracleHcmId = employee.PersonId,
            BadgeNumber = employee.BadgeNumber,
            DateOfBirth = employee.DateOfBirth,
            HireDate = employee.WorkRelationship?.StartDate ?? SqlDateTime.MinValue.Value.ToDateOnly(),
            TerminationDate = employee.WorkRelationship?.TerminationDate,
            Ssn = (employee.NationalIdentifier?.NationalIdentifierNumber ?? _faker.Person.Ssn()).ConvertSsnToInt(),
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

