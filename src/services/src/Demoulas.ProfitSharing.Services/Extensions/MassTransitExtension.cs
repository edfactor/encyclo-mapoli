using Demoulas.ProfitSharing.OracleHcm.Messaging;
using MassTransit;
using Microsoft.Extensions.Hosting;

    namespace Demoulas.ProfitSharing.Services.Extensions;

internal static class MassTransitExtension
{
    public static IHostApplicationBuilder ConfigureMassTransitServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<OracleHcmMessageConsumer>();

                x.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
            });


        return builder;
    }
}
