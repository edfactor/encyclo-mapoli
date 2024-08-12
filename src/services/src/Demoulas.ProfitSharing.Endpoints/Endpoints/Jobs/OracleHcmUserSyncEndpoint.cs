using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Contracts.Request.Job;
using Demoulas.ProfitSharing.Common.Contracts.Response.Job;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Services.Jobs;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Jobs;

public class OracleHcmUserSyncEndpoint : Endpoint<UserSyncRequestDto, UserSyncResponseDto>
{
    private readonly IEmployeeSyncJob _employeeSyncJob;

    public OracleHcmUserSyncEndpoint(IEmployeeSyncJob employeeSyncJob)
    {
        _employeeSyncJob = employeeSyncJob;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("oraclehcm/sync/user");
        Summary(s =>
        {
            s.Summary = "Queues a synchronization job with Oracle HCM for the specific user specified.";
            s.Description =
                "A synchronization job will begin the process of downloading updating the specific employee from OracleHCM and ensuring that the profit sharing database is fully synchronized for that individual.";
            s.ExampleRequest = new UserSyncRequestDto { BadgeNumber = 1_234_567 };
            s.ResponseExamples = new Dictionary<int, object> { { 200, new UserSyncResponseDto { Message = "Success Message" } } };
        });
        Group<JobsGroup>();
    }

    public override async Task<UserSyncResponseDto> ExecuteAsync(UserSyncRequestDto req, CancellationToken ct)
    {
       await _employeeSyncJob.SynchronizeEmployee(req.BadgeNumber, ct);
       return new UserSyncResponseDto { Message = "Synchronization Complete" };
    }
}
