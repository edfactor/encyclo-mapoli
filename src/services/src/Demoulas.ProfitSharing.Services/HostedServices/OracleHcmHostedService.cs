using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Microsoft.Extensions.Hosting;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Services.HostedServices;
public class OracleHcmHostedService : BackgroundService
{
    private readonly ISynchronizationService _synchronizationService;

    public OracleHcmHostedService(ISynchronizationService synchronizationService)
    {
        _synchronizationService = synchronizationService;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return _synchronizationService.SendSynchronizationRequest(
            new OracleHcmJobRequest
            {
                JobType = JobType.Constants.Delta, StartMethod = StartMethod.Constants.System, RequestedBy = nameof(StartMethod.Constants.System)
            }, cancellationToken);
    }
}
