using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Services.HostedServices;
using Demoulas.ProfitSharing.Services.Jobs;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Services.Extensions;
internal static class MassTransitExtension
{
    public static void ConfigureMassTransitServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
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
