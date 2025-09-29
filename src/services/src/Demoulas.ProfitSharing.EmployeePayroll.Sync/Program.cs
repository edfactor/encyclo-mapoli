using System.Runtime.InteropServices;
using Demoulas.Common.Contracts.Configuration;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.Common.Logging.Extensions;
using Demoulas.ProfitSharing.Common.Metrics;
using OpenTelemetry.Metrics;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Interceptors;
using Demoulas.ProfitSharing.OracleHcm.Extensions;
using Demoulas.ProfitSharing.Services.LogMasking;
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

builder.AddDatabaseServices((services, factoryRequests) =>
{
    // Register contexts without immediately resolving the interceptor
    factoryRequests.Add(ContextFactoryRequest.Initialize<ProfitSharingDbContext>("ProfitSharing",
        interceptorFactory: sp => [sp.GetRequiredService<AuditSaveChangesInterceptor>()]));
    factoryRequests.Add(ContextFactoryRequest.Initialize<ProfitSharingReadOnlyDbContext>("ProfitSharing"));
    factoryRequests.Add(ContextFactoryRequest.Initialize<DemoulasCommonDataContext>("ProfitSharing"));
});

builder.AddServiceDefaults(null, null);
builder.Services.AddOpenTelemetry().WithMetrics(m =>
{
    m.AddMeter(GlobalMeter.Name);
});

ElasticSearchConfig smartConfig = new ElasticSearchConfig();
builder.Configuration.Bind("Logging:Smart", smartConfig);

FileSystemLogConfig fileSystemLog = new FileSystemLogConfig();
builder.Configuration.Bind("Logging:FileSystem", fileSystemLog);

smartConfig.MaskingOperators = [
    new UnformattedSocialSecurityNumberMaskingOperator(),
    new SensitiveValueMaskingOperator()
];
builder.SetDefaultLoggerConfiguration(smartConfig, fileSystemLog);

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
GlobalMeter.InitializeFromServices(host.Services);
GlobalMeter.RegisterObservableGauges();
GlobalMeter.RecordDeploymentStartup();
await host.RunAsync().ConfigureAwait(false);
