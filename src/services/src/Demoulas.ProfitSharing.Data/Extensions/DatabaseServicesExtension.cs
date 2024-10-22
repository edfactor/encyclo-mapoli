using System.Diagnostics;
using System.Reflection;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Factories;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
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

                    if (dropAndRecreate)
                    {
                        // Get the location of the executing assembly
                        string assemblyPath = Assembly.GetExecutingAssembly().Location;

                        // Get the directory of the executing assembly
                        string? directoryPath = Path.GetDirectoryName(assemblyPath);
                        
                        string migrateScript =
                            await File.ReadAllTextAsync($"{directoryPath}{Path.DirectorySeparatorChar}ImportScripts{Path.DirectorySeparatorChar}SQL copy all from ready to smart ps.sql");
                        migrateScript = migrateScript.Replace("COMMIT ;", string.Empty).Trim();
                        await context.Database.ExecuteSqlRawAsync(migrateScript);
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
