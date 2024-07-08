using System.Diagnostics;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Data.Factories;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Data.Extensions;

public static class DatabaseServicesExtension
{
    public static IHostApplicationBuilder AddDatabaseServices(this IHostApplicationBuilder builder,
       IEnumerable<ContextFactoryRequest> contextFactoryRequests, bool runMigration = false)
    {
        if (builder.Services.Any(s => s.ServiceType == typeof(IProfitSharingDataContextFactory)))
        {
            throw new InvalidOperationException($"Service type {typeof(IProfitSharingDataContextFactory).FullName} is already registered.");
        }

        var factory = DataContextFactory.Initialize(builder, contextFactoryRequests: contextFactoryRequests);
        builder.Services.AddSingleton(factory);

        if (runMigration && Debugger.IsAttached)
        {
            factory.UseWritableContext(context =>
            {
                context.Database.Migrate();
                return Task.CompletedTask;
            });
        }

        return builder;
    }
}
