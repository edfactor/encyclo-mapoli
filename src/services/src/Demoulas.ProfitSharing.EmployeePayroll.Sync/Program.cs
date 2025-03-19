using System.Runtime.InteropServices;
using Demoulas.Common.Contracts.Configuration;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.Common.Logging.Extensions;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.OracleHcm.Extensions;
using Demoulas.Util.Extensions;

var builder = Host.CreateApplicationBuilder(args);

if (!builder.Environment.IsTestEnvironment())
{
    builder.Configuration
        .AddJsonFile($"credSettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddUserSecrets<Program>();
}
else
{
    builder.Configuration
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
}

List<ContextFactoryRequest> list =
[
    ContextFactoryRequest.Initialize<ProfitSharingDbContext>("ProfitSharing"),
    ContextFactoryRequest.Initialize<ProfitSharingReadOnlyDbContext>("ProfitSharing"),
    ContextFactoryRequest.Initialize<DemoulasCommonDataContext>("StoreInfo")
];

builder.AddDatabaseServices(list);

builder.AddServiceDefaults(null, null);

ElasticSearchConfig smartConfig = new ElasticSearchConfig();
builder.Configuration.Bind("Logging:Smart", smartConfig);

FileSystemLogConfig fileSystemLog = new FileSystemLogConfig();
builder.Configuration.Bind("Logging:FileSystem", fileSystemLog);

await builder.SetDefaultLoggerConfigurationAsync(smartConfig, fileSystemLog).ConfigureAwait(false);

builder.AddEmployeePayrollSyncService();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    builder.Services.AddWindowsService(options => { options.ServiceName = "Demoulas ProfitSharing Payroll Sync"; });
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    builder.Services.AddSystemd();
}

var host = builder.Build();
await host.RunAsync().ConfigureAwait(false);
