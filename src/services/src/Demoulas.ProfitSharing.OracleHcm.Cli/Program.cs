
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Jobs;
using Demoulas.StoreInfo.Entities.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;

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
            ContextFactoryRequest.Initialize<StoreInfoDbContext>("StoreInfo")
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
