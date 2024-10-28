using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Factories;

namespace Demoulas.ProfitSharing.Data.Cli;

#pragma warning disable S1118
public class Program
#pragma warning restore S1118
{
    public static async Task<int> Main(string[] args)
    {
#pragma warning disable S3928
        // Setup configuration to include command-line arguments
        var configuration = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();

        // Create root command
        var rootCommand = new RootCommand("CLI tool for database operations");

        // Define and set up "upgrade-db" command
        var upgradeDbCommand = new Command("upgrade-db", "Apply migrations to upgrade the database")
        {
            new Option<string>("--connection-name", "The name of the configuration property that holds the connection string")
        };

        upgradeDbCommand.SetHandler(async () =>
        {
            string? connectionName = configuration["connection-name"];
            if (string.IsNullOrEmpty(connectionName))
            {
                throw new ArgumentNullException(nameof(connectionName), "Connection name must be provided.");
            }

            HostApplicationBuilder builder = CreateHostBuilder(args);
            var list = new List<ContextFactoryRequest>
            {
                ContextFactoryRequest.Initialize<ProfitSharingDbContext>(connectionName)
            };

            var factory = DataContextFactory.Initialize(builder, contextFactoryRequests: list);
            await factory.UseWritableContext(context => context.Database.MigrateAsync());
        });

        // Define and set up "drop-recreate-db" command
        var dropRecreateDbCommand = new Command("drop-recreate-db", "Drop and recreate the database")
        {
            new Option<string>("--connection-name", "The name of the configuration property that holds the connection string")
        };

        dropRecreateDbCommand.SetHandler(async () =>
        {
            string? connectionName = configuration["connection-name"];
            if (string.IsNullOrEmpty(connectionName))
            {
                throw new ArgumentNullException(nameof(connectionName), "Connection name must be provided.");
            }

            HostApplicationBuilder builder = CreateHostBuilder(args);
            var list = new List<ContextFactoryRequest>
            {
                ContextFactoryRequest.Initialize<ProfitSharingDbContext>(connectionName)
            };

            var factory = DataContextFactory.Initialize(builder, contextFactoryRequests: list);
            await factory.UseWritableContext(async context =>
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
            });
        });

        // Define and set up "run-sql" command
        var runSqlCommand = new Command("run-sql", "Run a custom SQL script after migrations")
        {
            new Option<string>("--connection-name", "The name of the configuration property that holds the connection string"),
            new Option<string>("--sql-file", "The path to the custom SQL file")
        };

        runSqlCommand.SetHandler(async () =>
        {
            string? connectionName = configuration["connection-name"];
            string? sqlFile = configuration["sql-file"];
            if (string.IsNullOrEmpty(connectionName))
            {
                throw new ArgumentNullException(nameof(connectionName), "Connection name must be provided.");
            }

            if (string.IsNullOrEmpty(sqlFile))
            {

                throw new ArgumentNullException(nameof(sqlFile), "SQL file path must be provided.");
            }

            HostApplicationBuilder builder = CreateHostBuilder(args);
            var list = new List<ContextFactoryRequest>
            {
                ContextFactoryRequest.Initialize<ProfitSharingDbContext>(connectionName)
            };

            var factory = DataContextFactory.Initialize(builder, contextFactoryRequests: list);
            await factory.UseWritableContext(async context =>
            {
                string sqlCommand = await File.ReadAllTextAsync(sqlFile);
                sqlCommand = sqlCommand.Replace("COMMIT ;", string.Empty).Trim();
                return await context.Database.ExecuteSqlRawAsync(sqlCommand);
            });
        });

        // Add commands to root command
        rootCommand.AddCommand(upgradeDbCommand);
        rootCommand.AddCommand(dropRecreateDbCommand);
        rootCommand.AddCommand(runSqlCommand);

        // Invoke the root command
        return await rootCommand.InvokeAsync(args);
#pragma warning restore S3928
    }

    private static HostApplicationBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddUserSecrets<Program>();
        return builder;
    }
}
