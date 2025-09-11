using System.Threading.Channels;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Extensions;
using Demoulas.ProfitSharing.OracleHcm.Validators;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Demoulas.ProfitSharing.OracleHcm.Messaging;

internal class EmployeeSyncChannelConsumer : BackgroundService
{
    private readonly ChannelReader<MessageRequest<OracleEmployee[]>> _reader;
    private readonly OracleEmployeeValidator _employeeValidator;
    private readonly IDemographicsServiceInternal _demographicsService;
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly IFakeSsnService _fakeSsnService;
    private readonly IProfitSharingDataContextFactory _contextFactory;

    public EmployeeSyncChannelConsumer(
        Channel<MessageRequest<OracleEmployee[]>> channel,
        OracleEmployeeValidator employeeValidator,
        OracleHcmConfig oracleHcmConfig,
        IFakeSsnService fakeSsnService,
        IProfitSharingDataContextFactory contextFactory,
        IServiceScopeFactory scopeFactory)
    {
        _reader = channel.Reader;
        _employeeValidator = employeeValidator;
        using var scope = scopeFactory.CreateScope();
        _demographicsService = scope.ServiceProvider.GetRequiredService<IDemographicsServiceInternal>();
        _oracleHcmConfig = oracleHcmConfig;
        _fakeSsnService = fakeSsnService;
        _contextFactory = contextFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _reader.ReadAllAsync(stoppingToken).ConfigureAwait(false))
        {
            await ProcessMessage(message, stoppingToken).ConfigureAwait(false);
        }
    }

    private async Task ProcessMessage(MessageRequest<OracleEmployee[]> message, CancellationToken cancellationToken)
    {
        OracleEmployee[] employees = message.Body;
        DemographicsRequest[] requestDtoEnumerable = await ConvertToRequestDto(employees, message.UserId, cancellationToken).ConfigureAwait(false);
        await _demographicsService.AddDemographicsStreamAsync(requestDtoEnumerable, _oracleHcmConfig.Limit, cancellationToken).ConfigureAwait(false);
    }

    private async Task<DemographicsRequest[]> ConvertToRequestDto(OracleEmployee[] employees,
        string requestedBy, CancellationToken cancellationToken)
    {
        async Task<Dictionary<long, int>> GetFakeSsns(List<long> oracleHcmIds)
        {
            Dictionary<long, int> empSsnDic = await _contextFactory.UseReadOnlyContext(c =>
            {
                return c.Demographics.Where(d => oracleHcmIds.Contains(d.OracleHcmId))
                    .Select(d => new { d.OracleHcmId, d.Ssn })
                    .ToDictionaryAsync(d => d.OracleHcmId, d => d.Ssn, cancellationToken);
            }).ConfigureAwait(false);

            var missingIds = oracleHcmIds.Where(id => !empSsnDic.ContainsKey(id)).ToList();
            var newSsns = await _fakeSsnService.GenerateFakeSsnBatchAsync(missingIds.Count, cancellationToken).ConfigureAwait(false);
            for (int i = 0; i < missingIds.Count; i++)
            {
                empSsnDic.Add(missingIds[i], newSsns[i]);
            }
            return empSsnDic;
        }

        var empSsnDic = await GetFakeSsns(employees.Select(e => e.PersonId).ToList()).ConfigureAwait(false);
        List<DemographicsRequest> requests = new();
        foreach (var employee in employees)
        {
            int badgeNumber = employee?.BadgeNumber ?? 0;
            if (employee == null || badgeNumber == 0)
            {
                continue;
            }
            ValidationResult? result = await _employeeValidator.ValidateAsync(employee!, cancellationToken).ConfigureAwait(false);
            if (!result.IsValid)
            {
                await _demographicsService.AuditError(badgeNumber, employee?.PersonId ?? 0, result.Errors, requestedBy, cancellationToken).ConfigureAwait(false);
                continue;
            }
            var dr = new DemographicsRequest
            {
                OracleHcmId = employee.PersonId,
                BadgeNumber = employee.BadgeNumber,
                DateOfBirth = employee.DateOfBirth,
                HireDate = employee.WorkRelationship?.StartDate ?? ReferenceData.DsmMinValue,
                TerminationDate = employee.WorkRelationship?.TerminationDate,
                Ssn = employee.NationalIdentifier?.NationalIdentifierNumber.ConvertSsnToInt() ?? empSsnDic[employee.PersonId],
                StoreNumber = employee.WorkRelationship?.Assignment.LocationCode ?? 0,
                DepartmentId = employee.WorkRelationship?.Assignment.GetDepartmentId() ?? 0,
                PayClassificationId = employee.WorkRelationship?.Assignment.JobCode ?? 0,
                EmploymentTypeCode = employee.WorkRelationship?.Assignment.GetEmploymentType() ?? char.MinValue,
                PayFrequencyId = employee.WorkRelationship?.Assignment.GetPayFrequency() ?? byte.MinValue,
                EmploymentStatusId = employee.WorkRelationship?.TerminationDate == null ? EmploymentStatus.Constants.Active : EmploymentStatus.Constants.Terminated,
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
            requests.Add(dr);
        }
        return requests.ToArray();
    }
}
