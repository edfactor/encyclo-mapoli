using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Services.HostedServices;
using Demoulas.ProfitSharing.Services.Jobs;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Services.Extensions;
internal static class MassTransitExtension
{
    public static void ConfigureMassTransitServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumer<JobConsumer>();

            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        if (!builder.Environment.IsTestEnvironment())
        {
            _ = builder.Services.AddHostedService<OracleHcmHostedService>();
        }
    }
}
