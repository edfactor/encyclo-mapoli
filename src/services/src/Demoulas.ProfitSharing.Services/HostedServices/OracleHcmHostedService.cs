using Demoulas.ProfitSharing.Common.ActivitySources;
using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Services.HostedServices;
public class OracleHcmHostedService : BackgroundService
{
    private readonly IBus _bus;
    private readonly IHostEnvironment _hostEnvironment;

    public OracleHcmHostedService(IBus bus, IHostEnvironment hostEnvironment)
    {
        _bus = bus;
        _hostEnvironment = hostEnvironment;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var message = new MessageRequest<OracleHcmJobRequest>
        {
            ApplicationName = _hostEnvironment.ApplicationName,
            Body = new OracleHcmJobRequest
            {
                JobType = JobType.Constants.Delta,
                StartMethod = StartMethod.Constants.System,
                RequestedBy = nameof(StartMethod.Constants.System)
            }
        };

        using var activity = OracleHcmActivitySource.Instance.StartActivity(name: "Sync Employees from OracleHCM - Application Startup", kind: ActivityKind.Internal);
        return _bus.Publish(message: message, cancellationToken: cancellationToken);
    }
}
