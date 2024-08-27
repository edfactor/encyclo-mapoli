using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.OracleHcm.Jobs;
using Demoulas.ProfitSharing.Services.HostedServices;
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

            x.AddConsumer<OracleHcmJobConsumer>();

            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        if (!builder.Environment.IsTestEnvironment())
        {
            _ = builder.Services.AddHostedService<StartupHostedService>();
        }
    }
}
