using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.OracleHcm;
using Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
using Quartz;

namespace Demoulas.ProfitSharing.Services.Jobs;
internal sealed class EmployeeSyncJob : IJob
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

    public Task Execute(IJobExecutionContext context)
    {
        var oracleHcmEmployees = _oracleDemographicsService.GetAllEmployees(context.CancellationToken);
        var requestDtoEnumerable = ConvertToRequestDto(oracleHcmEmployees);
        return _demographicsService.AddDemographicsStream(requestDtoEnumerable, _oracleHcmConfig.Limit, context.CancellationToken);
    }

    private async IAsyncEnumerable<DemographicsRequestDto> ConvertToRequestDto(IAsyncEnumerable<OracleEmployee?> asyncEnumerable)
    {
        await foreach (OracleEmployee? employee in asyncEnumerable)
        {
            if (employee?.Address == null)
            {
                continue;
            }

            yield return new DemographicsRequestDto
            {
                OracleHcmId = employee.PersonId,
                BadgeNumber = employee.BadgeNumber,
                DateOfBirth = employee.DateOfBirth,
                FirstName = employee.Name.FirstName,
                LastName = employee.Name.LastName,
                FullName = employee.Name.DisplayName,


                SSN = 0,
                StoreNumber = 0,
                DepartmentId = Department.Constants.Beer_And_Wine,
                PayClassificationId = PayClassification.Constants.ApprMeatCutters,
                HireDate = employee.Name.EffectiveStartDate.ToDateOnly(),
                EmploymentTypeCode = EmploymentType.Constants.PartTime,
                PayFrequencyId = PayFrequency.Constants.Weekly,
                EmploymentStatusId = EmploymentStatus.Constants.Active,
                GenderCode = Gender.Constants.Other,


                ContactInfo = new ContactInfoRequestDto
                {
                    PhoneNumber = employee.Phone?.PhoneNumber,
                    EmailAddress = employee.Email?.EmailAddress
                },
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
