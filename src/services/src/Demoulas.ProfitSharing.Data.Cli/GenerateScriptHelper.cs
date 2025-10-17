using System.CommandLine;
using Demoulas.Common.Contracts.Configuration;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.Common.Logging.Extensions;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Factories;
using Demoulas.ProfitSharing.Data.Interceptors;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.ProfitSharing.Services.LogMasking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Data.Cli;

internal static class GenerateScriptHelper
{
    public static Command CreateGenerateUpgradeScriptCommand(
        IConfiguration configuration,
        string[] args,
        List<Option> commonOptions)
    {
        var generateScriptCommand = new Command("generate-upgrade-script", "Generate a SQL script to upgrade the database to the latest migration");

        // Add any common options you have
        commonOptions.ForEach(o => generateScriptCommand.Add(o));

        // Note: handler execution is performed by the caller (Program) to avoid depending on
        // System.CommandLine handler extension APIs which may change between previews.

        return generateScriptCommand;
    }

    // Overload that also exposes the IServiceProvider so callers can resolve additional services via DI
    internal static async Task ExecuteWithDbContext(
        IConfiguration configuration,
        string[] args,
        Func<IServiceProvider, ProfitSharingDbContext, Task> action)
    {
        string? connectionName = configuration["connection-name"];

        // Some run modes (for example when running via certain launch/profile tooling)
        // may not expose the launch profile's commandLineArgs through the "args" passed
        // to Main or the IConfiguration built from AddCommandLine. As a robustness
        // measure, fall back to scanning the provided args[] for --connection-name
        // (either "--connection-name value" or "--connection-name=value" forms).
        if (string.IsNullOrEmpty(connectionName) && args != null && args.Length > 0)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var a = args[i];
                if (string.Equals(a, "--connection-name", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                {
                    connectionName = args[i + 1];
                    break;
                }
                if (a.StartsWith("--connection-name=", StringComparison.OrdinalIgnoreCase))
                {
                    connectionName = a.Substring("--connection-name=".Length);
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(connectionName))
        {
            // Provide a clear error when the required connection-name is not supplied.
            // Callers should pass the connection name via the `--connection-name` option
            // or set an equivalent environment variable (e.g., when running in CI).
            throw new ArgumentException("Missing required configuration: connection-name. Please provide --connection-name <name> or set the corresponding environment variable.");
        }

        HostApplicationBuilder builder = CreateHostBuilder(args ?? Array.Empty<string>());

        // Configure logging - configuration read from SmartLogging section in appsettings
        builder.SetDefaultLoggerConfiguration(config =>
        {
            config.MaskingOperators = [
                new UnformattedSocialSecurityNumberMaskingOperator(),
                new SensitiveValueMaskingOperator()
            ];
        });

        builder.AddDatabaseServices((services, factoryRequests) =>
        {
            // Register contexts without immediately resolving the interceptor
            factoryRequests.Add(ContextFactoryRequest.Initialize<ProfitSharingDbContext>("ProfitSharing",
                interceptorFactory: sp =>
                [
                    sp.GetRequiredService<AuditSaveChangesInterceptor>(),
                    sp.GetRequiredService<BeneficiarySaveChangesInterceptor>(),
                    sp.GetRequiredService<BeneficiaryContactSaveChangesInterceptor>()
                ]));
            factoryRequests.Add(ContextFactoryRequest.Initialize<ProfitSharingReadOnlyDbContext>("ProfitSharing"));
            factoryRequests.Add(ContextFactoryRequest.Initialize<DemoulasCommonDataContext>("ProfitSharing"));
        });
        builder.AddProjectServices();

        _ = builder.Services.AddScoped<IAppUser, DummyUser>();
        _ = builder.Services.AddScoped<RebuildEnrollmentAndZeroContService>();

        await using var sp = builder.Services.BuildServiceProvider();
        await using var scope = sp.CreateAsyncScope();
        var provider = scope.ServiceProvider;
        var context = provider.GetRequiredService<ProfitSharingDbContext>();
        await action(provider, context);
    }


    private static HostApplicationBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        builder.Configuration.AddUserSecrets<Program>();
        builder.Configuration.AddEnvironmentVariables();
        return builder;
    }

}
