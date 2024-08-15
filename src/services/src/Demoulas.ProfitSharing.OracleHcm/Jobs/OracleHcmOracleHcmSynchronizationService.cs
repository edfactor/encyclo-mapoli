using System.Diagnostics;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Data.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.OracleHcm.Jobs;

public sealed class OracleHcmOracleHcmSynchronizationService : IOracleHcmSynchronizationService
{
    private readonly IBus _bus;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public OracleHcmOracleHcmSynchronizationService(IBus bus, IHostEnvironment hostEnvironment, IProfitSharingDataContextFactory dataContextFactory)
    {
        _bus = bus;
        _hostEnvironment = hostEnvironment;
        _dataContextFactory = dataContextFactory;
    }

    public async Task<bool> SendSynchronizationRequest(OracleHcmJobRequest request, CancellationToken cancellationToken = default)
    {
        var message = new MessageRequest<OracleHcmJobRequest> { ApplicationName = _hostEnvironment.ApplicationName, Body = request };

        bool jobTypeIsAlreadyRunning = await _dataContextFactory.UseReadOnlyContext(c =>
            {
                return c.Jobs.AnyAsync(j => j.JobTypeId == request.JobType 
                                            && j.JobStatusId == JobStatus.Constants.Running
                                            && j.Started > DateTime.Now.AddDays(-1),
                    cancellationToken: cancellationToken);
            }
        );

        if (jobTypeIsAlreadyRunning)
        {
            return false;
        }

        _ = OracleHcmActivitySource.Instance.StartActivity(name: "Full Sync Employees from OracleHCM", kind: ActivityKind.Internal);
        await _bus.Publish(message: message, cancellationToken: cancellationToken);

        return true;
    }
}
