using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Microsoft.Extensions.Hosting;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Services.HostedServices;
public class StartupHostedService : BackgroundService
{
    private readonly IOracleHcmSynchronizationService _oracleHcmSynchronizationService;

    public StartupHostedService(IOracleHcmSynchronizationService oracleHcmSynchronizationService)
    {
        _oracleHcmSynchronizationService = oracleHcmSynchronizationService;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return _oracleHcmSynchronizationService.SendSynchronizationRequest(
            new OracleHcmJobRequest
            {
                JobType = JobType.Constants.Delta, StartMethod = StartMethod.Constants.System, RequestedBy = nameof(StartMethod.Constants.System)
            }, cancellationToken);
    }
}
