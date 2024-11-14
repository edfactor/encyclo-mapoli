using System.Threading;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Contracts.Response.Job;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using MassTransit;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Jobs;

public class OracleHcmFullSyncEndpoint : EndpointWithoutRequest<SendMessageResponse>
{
    private readonly IBus _bus;
    private readonly IAppUser _appUser;

    public OracleHcmFullSyncEndpoint(IBus bus, IAppUser appUser)
    {
        _bus = bus;
        _appUser = appUser;
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

    public override Task HandleAsync(CancellationToken ct)
    {
        var message = new MessageRequest<OracleHcmJobRequest>
        {
            ApplicationName = Env.ApplicationName,
            Body = new OracleHcmJobRequest
            {
                JobType = JobType.Constants.EmployeeSyncFull,
                StartMethod = StartMethod.Constants.OnDemand,
                RequestedBy = _appUser.UserName ?? "UnKnown User"
            }
        };

        return _bus.Publish(message: message, cancellationToken: ct);
    }
}

