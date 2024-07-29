using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Services.HostedServices;
public class OracleHcmHostedService : BackgroundService
{
    private readonly IBus _bus;

    public OracleHcmHostedService(IBus bus)
    {
        _bus = bus;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var message = new MessageRequest<OracleHcmJobRequest>
        {
            Body = new OracleHcmJobRequest { JobType = "Delta", StartMethod = "System", RequestedBy = "System" }
        };

        return _bus.Publish(message, cancellationToken);
    }
}
