using System.Runtime.InteropServices;
using Demoulas.Common.Contracts.Configuration;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.Common.Logging.Extensions;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Metrics;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Interceptors;
using Demoulas.ProfitSharing.OracleHcm.Extensions;
using Demoulas.ProfitSharing.Services.LogMasking;
using Demoulas.Util.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

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
        interceptorFactory: sp => [
            sp.GetRequiredService<AuditSaveChangesInterceptor>(),
            sp.GetRequiredService<BeneficiarySaveChangesInterceptor>(),
            sp.GetRequiredService<BeneficiaryContactSaveChangesInterceptor>()
        ]));
    factoryRequests.Add(ContextFactoryRequest.Initialize<ProfitSharingReadOnlyDbContext>("ProfitSharing"));
    factoryRequests.Add(ContextFactoryRequest.Initialize<IDemoulasCommonWarehouseContext, DemoulasCommonWarehouseContext>("Warehouse"));
});

builder.AddServiceDefaults(null, null);
builder.Services.AddOpenTelemetry().WithMetrics(m =>
{
    m.AddMeter(GlobalMeter.Name);
});

// Configure logging - configuration read from SmartLogging section in appsettings
LoggingConfig logConfig = new();
builder.Configuration.Bind("SmartLogging", logConfig);

logConfig.MaskingOperators = [
    new UnformattedSocialSecurityNumberMaskingOperator(),
    new SensitiveValueMaskingOperator()
];

builder.SetDefaultLoggerConfiguration(logConfig);

builder.AddProcessWatchdog();

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

