using System.CommandLine;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Factories;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Extensions;
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
        commonOptions.ForEach(generateScriptCommand.AddOption);

        generateScriptCommand.SetHandler(async _ =>
        {
            var outputPath = configuration["output-file"];

            // Use your existing helper method to retrieve a DbContext.
            // The ExecuteWithDbContext signature presumably looks something like:
            //   Task ExecuteWithDbContext(IConfiguration config, string[] args, Func<DbContext, Task> action)
            await ExecuteWithDbContext(configuration, args, async dbContext =>
            {
                var migrator = dbContext.GetService<IMigrator>();

                var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync();
                var currentMigration = appliedMigrations.LastOrDefault("0");

                Console.WriteLine($"Current DB version: {currentMigration}");

                var hasPendingChanges = await dbContext.Database.GetPendingMigrationsAsync();
                if (!hasPendingChanges.Any())
                {
                    Console.WriteLine("Database is current. No changes need be applied.");
                    return;
                }

                // Generate the migration script from "0" (initial) to the latest.
                // If you only want pending migrations, you can detect the last applied migration and use that instead.
                var script = migrator.GenerateScript(
                    fromMigration: currentMigration,
                    toMigration: null, // null means "to the latest"
                    options: MigrationsSqlGenerationOptions.Idempotent | MigrationsSqlGenerationOptions.Script
                );

                if (!string.IsNullOrWhiteSpace(outputPath))
                {
                    // Validate the output path to prevent path traversal
                    var fullPath = Path.GetFullPath(outputPath);
                    var basePath = Path.GetFullPath(AppContext.BaseDirectory);

                    if (!fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Invalid output path.");
                    }

                    var directory = Path.GetDirectoryName(outputPath);
                    if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Write the script to a file
                    await File.WriteAllTextAsync(fullPath, script);
                    Console.WriteLine($"SQL upgrade script generated at: {fullPath}");
                }
                else
                {
                    // Print the script to the console
                    Console.WriteLine("Generated SQL Script:");
                    Console.WriteLine(script);
                }
            });
        });

        return generateScriptCommand;
    }

    internal static async Task ExecuteWithDbContext(IConfiguration configuration, string[] args, Func<ProfitSharingDbContext, Task> action)
    {
        string? connectionName = configuration["connection-name"];
        if (string.IsNullOrEmpty(connectionName))
        {
#pragma warning disable S112
            throw new NullReferenceException(nameof(connectionName));
#pragma warning restore S112
        }

        HostApplicationBuilder builder = CreateHostBuilder(args);
        _ = builder.AddProjectServices();
        _ = builder.Services.AddScoped<IAppUser, DummyUser>();

        var list = new List<ContextFactoryRequest> { ContextFactoryRequest.Initialize<ProfitSharingDbContext>(connectionName) };
        _ = DataContextFactory.Initialize(builder, contextFactoryRequests: list);

        await using var context = builder.Services.BuildServiceProvider().GetRequiredService<ProfitSharingDbContext>();
        await action(context);
#pragma warning restore S3928
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
        _ = builder.AddProjectServices();
        _ = builder.Services.AddScoped<IAppUser, DummyUser>();
        _ = builder.Services.AddScoped<Demoulas.ProfitSharing.Services.RebuildEnrollmentAndZeroContService>();

        var list = new List<ContextFactoryRequest> { ContextFactoryRequest.Initialize<ProfitSharingDbContext>(connectionName) };
        _ = DataContextFactory.Initialize(builder, contextFactoryRequests: list);

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
