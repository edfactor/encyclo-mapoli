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
        if (string.IsNullOrEmpty(connectionName))
        {
#pragma warning disable S112
            throw new NullReferenceException(nameof(connectionName));
#pragma warning restore S112
        }

        HostApplicationBuilder builder = CreateHostBuilder(args);


        ElasticSearchConfig smartConfig = new ElasticSearchConfig();
        builder.Configuration.Bind("Logging:Smart", smartConfig);

        FileSystemLogConfig fileSystemLog = new FileSystemLogConfig();
        builder.Configuration.Bind("Logging:FileSystem", fileSystemLog);

        smartConfig.MaskingOperators = [
            new UnformattedSocialSecurityNumberMaskingOperator(),
            new SensitiveValueMaskingOperator()
        ];
        builder.SetDefaultLoggerConfiguration(smartConfig, fileSystemLog);

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
        builder.Configuration.AddUserSecrets<Program>();
        builder.Configuration.AddEnvironmentVariables();
        return builder;
    }

}
