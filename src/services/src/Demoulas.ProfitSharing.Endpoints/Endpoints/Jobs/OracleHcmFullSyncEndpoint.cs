using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Contracts.Response.Job;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Jobs;

public class OracleHcmFullSyncEndpoint : EndpointWithoutRequest<SendMessageResponse>
{
    private readonly ISynchronizationService _synchronizationService;

    public OracleHcmFullSyncEndpoint(ISynchronizationService synchronizationService)
    {
        _synchronizationService = synchronizationService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("oraclehcm/sync/full");
        Summary(s =>
        {
            s.Summary = "Queues a full synchronization job with Oracle HCM.";
            s.Description =
                "A full synchronization job will begin the process of downloading all employees from OracleHCM and ensuring that the profit sharing database is fully synchronized.";
            s.ResponseExamples = new Dictionary<int, object> { { 202, new SendMessageResponse() } };
        });
        Group<JobsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        bool jobStarted = await _synchronizationService.SendSynchronizationRequest(
            new OracleHcmJobRequest { JobType = JobType.Constants.Full, StartMethod = StartMethod.Constants.OnDemand, RequestedBy = "Not Implemented" }, ct);

        var message = new SendMessageResponse();

        if (!jobStarted)
        {
            message.Message = "Unable to queue message";
        }

        await SendAsync(new SendMessageResponse(), 202, ct);
    }
}
