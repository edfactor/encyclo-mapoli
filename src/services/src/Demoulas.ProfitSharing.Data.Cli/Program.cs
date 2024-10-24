using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Factories;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Data.Cli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Root command
        var rootCommand = new RootCommand
            {
                new Option<string>(
                    "--connection-name",
                    "The name of the configuration property that holds the connection string for the database"
                ),
                new Option<string>("--sql-file", "The path to the custom SQL file"),
            };

        var builder = Host.CreateApplicationBuilder(args);
        List<ContextFactoryRequest> list = new List<ContextFactoryRequest>
        {
            ContextFactoryRequest.Initialize<ProfitSharingDbContext>("ProfitSharing")
        };

        IProfitSharingDataContextFactory factory = DataContextFactory.Initialize(builder, contextFactoryRequests: list);

        // Define the "upgrade-db" command
        var upgradeDbCommand = new Command("upgrade-db", "Apply migrations to upgrade the database")
        {
            Handler = CommandHandler.Create<string>((_) =>
            {
                return factory.UseWritableContext(context => context.Database.MigrateAsync());
            })
        };

        // Define the "drop-recreate-db" command
        var dropRecreateDbCommand = new Command("drop-recreate-db", "Drop and recreate the database")
        {
            Handler = CommandHandler.Create<string>((_) =>
            {
                return factory.UseWritableContext(context => context.Database.EnsureDeletedAsync());
            })
        };

        // Define the "run-sql" command
        var runSqlCommand = new Command("run-sql", "Run a custom SQL script after migrations")
        {
            Handler = CommandHandler.Create<string, string>((_, sqlFile) =>
            {
                return factory.UseWritableContext(context =>
                {
                    sqlFile = sqlFile.Replace("COMMIT ;", string.Empty).Trim();
                    return context.Database.ExecuteSqlRawAsync(sqlFile);
                });
            })
        };

        // Add the commands to the root command
        rootCommand.AddCommand(dropRecreateDbCommand);
        rootCommand.AddCommand(upgradeDbCommand);
        rootCommand.AddCommand(runSqlCommand);

        return await rootCommand.InvokeAsync(args);
    }
}
