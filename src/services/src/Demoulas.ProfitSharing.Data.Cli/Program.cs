using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Factories;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Data.Cli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Create root command
        var rootCommand = new RootCommand("CLI tool for database operations");

        // Define the "upgrade-db" command
        var upgradeDbCommand = new Command("upgrade-db", "Apply migrations to upgrade the database")
        {
            new Option<string>("--connection-name", "The name of the configuration property that holds the connection string")
        };

        // Set handler for the "upgrade-db" command
        upgradeDbCommand.SetHandler(async (string connectionName) =>
        {
            var builder = Host.CreateApplicationBuilder(args);
            List<ContextFactoryRequest> list = [ContextFactoryRequest.Initialize<ProfitSharingDbContext>(connectionName)];

            IProfitSharingDataContextFactory factory = DataContextFactory.Initialize(builder, contextFactoryRequests: list);
            await factory.UseWritableContext(context => context.Database.MigrateAsync());
        }, new Option<string>("--connection-name"));

        // Define the "drop-recreate-db" command
        var dropRecreateDbCommand = new Command("drop-recreate-db", "Drop and recreate the database")
        {
            new Option<string>("--connection-name", "The name of the configuration property that holds the connection string")
        };

        // Set handler for the "drop-recreate-db" command
        dropRecreateDbCommand.SetHandler(async (string connectionName) =>
        {
            var builder = Host.CreateApplicationBuilder(args);
            List<ContextFactoryRequest> list = [ContextFactoryRequest.Initialize<ProfitSharingDbContext>(connectionName)];

            IProfitSharingDataContextFactory factory = DataContextFactory.Initialize(builder, contextFactoryRequests: list);

            await factory.UseWritableContext(async context =>
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
            });
        }, new Option<string>("--connection-name"));

        // Define the "run-sql" command
        var runSqlCommand = new Command("run-sql", "Run a custom SQL script after migrations")
        {
            new Option<string>("--connection-name", "The name of the configuration property that holds the connection string"),
            new Option<string>("--sql-file", "The path to the custom SQL file")
        };

        // Set handler for the "run-sql" command
        runSqlCommand.SetHandler(async (string connectionName, string sqlFile) =>
        {
            var builder = Host.CreateApplicationBuilder(args);
            List<ContextFactoryRequest> list = [ContextFactoryRequest.Initialize<ProfitSharingDbContext>(connectionName)];

            IProfitSharingDataContextFactory factory = DataContextFactory.Initialize(builder, contextFactoryRequests: list);

            await factory.UseWritableContext(context =>
            {
                sqlFile = sqlFile.Replace("COMMIT ;", string.Empty).Trim();
                return context.Database.ExecuteSqlRawAsync(sqlFile);
            });
        }, new Option<string>("--connection-name"), new Option<string>("--sql-file"));

        // Add commands to root command
        rootCommand.AddCommand(upgradeDbCommand);
        rootCommand.AddCommand(dropRecreateDbCommand);
        rootCommand.AddCommand(runSqlCommand);

        // Invoke the root command
        return await rootCommand.InvokeAsync(args);
    }
}
