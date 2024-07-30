using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Services.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.OracleHcm.Cli;

public class Program
{
    public static async Task Main()
    {
        IHostApplicationBuilder builder = new HostApplicationBuilder();
        builder.Configuration.AddUserSecrets<Program>();

        List<ContextFactoryRequest> list = new List<ContextFactoryRequest>
        {
            ContextFactoryRequest.Initialize<ProfitSharingDbContext>("ProfitSharing"),
            ContextFactoryRequest.Initialize<ProfitSharingReadOnlyDbContext>("ProfitSharing"),
            ContextFactoryRequest.Initialize<DemoulasCommonDataContext>("StoreInfo")
        };
        await builder.AddDatabaseServices(list, true, true);
        Services.Extensions.ServicesExtension.AddProjectServices(builder);
        ServiceProvider provider = builder.Services.BuildServiceProvider();

        var syncJob = provider.GetRequiredService<EmployeeSyncJob>();

        await syncJob.SynchronizeEmployees(CancellationToken.None);

        await Task.CompletedTask;

        Console.ReadLine();
    }

   
}
