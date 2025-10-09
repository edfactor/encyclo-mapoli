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
    private readonly OracleHcmConfig _oracleHcmConfig;
    private readonly IFakeSsnService _fakeSsnService;
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly IServiceScopeFactory _scopeFactory;

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
        _oracleHcmConfig = oracleHcmConfig;
        _fakeSsnService = fakeSsnService;
        _contextFactory = contextFactory;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var demographicsService = scope.ServiceProvider.GetRequiredService<IDemographicsServiceInternal>();

        await foreach (var message in _reader.ReadAllAsync(stoppingToken).ConfigureAwait(false))
        {
            await ProcessMessage(message, demographicsService, stoppingToken).ConfigureAwait(false);
        }
    }

    private async Task ProcessMessage(MessageRequest<OracleEmployee[]> message, IDemographicsServiceInternal demographicsService, CancellationToken cancellationToken)
    {
        OracleEmployee[] employees = message.Body;
        DemographicsRequest[] requestDtoEnumerable = await ConvertToRequestDto(employees, message.UserId, cancellationToken).ConfigureAwait(false);
        await demographicsService.AddDemographicsStreamAsync(requestDtoEnumerable, _oracleHcmConfig.Limit, cancellationToken).ConfigureAwait(false);
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
            }, cancellationToken).ConfigureAwait(false);

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
                using var scope = _scopeFactory.CreateScope();
                var demographicsService = scope.ServiceProvider.GetRequiredService<IDemographicsServiceInternal>();

                await demographicsService.AuditError(badgeNumber, employee?.PersonId ?? 0, result.Errors, requestedBy, cancellationToken).ConfigureAwait(false);
                continue;
            }

            var dr = employee.CreateDemographicsRequest(empSsnDic);
            requests.Add(dr);
        }
        return requests.ToArray();
    }
}
