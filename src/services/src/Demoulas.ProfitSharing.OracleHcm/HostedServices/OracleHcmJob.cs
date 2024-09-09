using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Quartz;


namespace Demoulas.ProfitSharing.OracleHcm.HostedServices;
public class OracleHcmJob : IJob
{
    private readonly IOracleHcmSynchronizationService _oracleHcmSynchronizationService;

    public OracleHcmJob(IOracleHcmSynchronizationService oracleHcmSynchronizationService)
    {
        _oracleHcmSynchronizationService = oracleHcmSynchronizationService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context.CancellationToken;

        await _oracleHcmSynchronizationService.SendSynchronizationRequest(
            new OracleHcmJobRequest
            {
                JobType = JobType.Constants.Delta,
                StartMethod = StartMethod.Constants.System,
                RequestedBy = nameof(StartMethod.Constants.System)
            }, cancellationToken);
    }
}
