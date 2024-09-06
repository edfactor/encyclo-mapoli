using System.Diagnostics;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Factories;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Data.Extensions;

public static class DatabaseServicesExtension
{
    public static async Task<IHostApplicationBuilder> AddDatabaseServices(this IHostApplicationBuilder builder,
        IEnumerable<ContextFactoryRequest> contextFactoryRequests, bool runMigration = false, bool dropAndRecreate = false)
    {
        if (builder.Services.Any(s => s.ServiceType == typeof(IProfitSharingDataContextFactory)))
        {
            throw new InvalidOperationException($"Service type {typeof(IProfitSharingDataContextFactory).FullName} is already registered.");
        }

        List<ContextFactoryRequest> factoryRequests = contextFactoryRequests.ToList();
        IProfitSharingDataContextFactory factory = DataContextFactory.Initialize(builder, contextFactoryRequests: factoryRequests);
        _ = builder.Services.AddSingleton(factory);

        if (builder.Environment.IsTestEnvironment())
        {
            return builder;
        }

        if (runMigration && Debugger.IsAttached)
        {
            await factory.UseWritableContext(async context =>
            {
                try
                {
                    if (dropAndRecreate)
                    {
                        await context.Database.EnsureDeletedAsync();
                    }

                    IEnumerable<string> pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        await context.Database.MigrateAsync();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }

        return builder;
    }
}
