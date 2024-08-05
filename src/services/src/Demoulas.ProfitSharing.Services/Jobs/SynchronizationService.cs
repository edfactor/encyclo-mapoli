using System.Diagnostics;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Data.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Services.Jobs;

public sealed class SynchronizationService : ISynchronizationService
{
    private readonly IBus _bus;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public SynchronizationService(IBus bus, IHostEnvironment hostEnvironment, IProfitSharingDataContextFactory dataContextFactory)
    {
        _bus = bus;
        _hostEnvironment = hostEnvironment;
        _dataContextFactory = dataContextFactory;
    }

    public async Task<bool> SendSynchronizationRequest(OracleHcmJobRequest request, CancellationToken cancellationToken = default)
    {
        var message = new MessageRequest<OracleHcmJobRequest> { ApplicationName = _hostEnvironment.ApplicationName, Body = request };

        bool isJobTypeAlreadyRunning = await _dataContextFactory.UseReadOnlyContext(c =>
            {
                return c.Jobs.AnyAsync(j => j.JobTypeId == request.JobType && j.JobStatusId == JobStatus.Constants.Running,
                    cancellationToken: cancellationToken);
            }
        );

        if (isJobTypeAlreadyRunning)
        {
            return false;
        }

        _ = OracleHcmActivitySource.Instance.StartActivity(name: "Sync Employees from OracleHCM - Application Startup", kind: ActivityKind.Internal);
        await _bus.Publish(message: message, cancellationToken: cancellationToken);

        return true;
    }
}
