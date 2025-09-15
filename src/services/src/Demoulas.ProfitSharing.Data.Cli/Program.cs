using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Text;
using Demoulas.Common.Data.Services.Entities.Contexts.EntityMapping.Data;
using Demoulas.ProfitSharing.Data.Cli.DiagramServices;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.Data.Cli;

#pragma warning disable S1118
public sealed class Program
#pragma warning restore S1118
{
    public static Task<int> Main(string[] args)
    {
#pragma warning disable S3928
        var configuration = new ConfigurationBuilder()
            .AddCommandLine(args)
            .AddEnvironmentVariables()
            .Build();

        var rootCommand = new RootCommand("CLI tool for database operations");

        var commonOptions = new List<Option>
        {
            new Option<string>("--connection-name", "The name of the configuration property that holds the connection string"),
            new Option<string>("--sql-file", "The path to the custom SQL file"),
            new Option<string>("--source-schema", "Name of the schema that is being used as the source database"),
            new Option<string>("--output-file", "The path to save the file (if applicable)")
        };

        var upgradeDbCommand = new Command("upgrade-db", "Apply migrations to upgrade the database");
        commonOptions.ForEach(upgradeDbCommand.AddOption);


        upgradeDbCommand.SetHandler(async () =>
        {
            await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async (_, context) =>
            {
                await context.Database.MigrateAsync();

                // Step 1: Get all existing AccountingPeriods from the database
                var existingIds = await context.AccountingPeriods
                    .Select(p => p.WeekendingDate) // Assuming 'Id' is the primary key
                    .ToListAsync();

                // Step 2: Find records that are NOT in the database
                var newRecords = CaldarRecordSeeder.Records
                    .Where(p => !existingIds.Contains(p.WeekendingDate))
                    .ToList();

                // Step 3: Insert only new records
                if (newRecords.Any())
                {
                    context.AccountingPeriods.AddRange(newRecords);
                    await context.SaveChangesAsync();
                }

                await GatherSchemaStatistics(context);
            });
        });

        var dropRecreateDbCommand = new Command("drop-recreate-db", "Drop and recreate the database");
        commonOptions.ForEach(dropRecreateDbCommand.AddOption);

        dropRecreateDbCommand.SetHandler(async () =>
        {
            await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async (_, context) =>
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
            });
        });

        var runSqlCommand = new Command("import-from-ready", "Run a custom SQL script after migrations");
        commonOptions.ForEach(runSqlCommand.AddOption);

        runSqlCommand.SetHandler(async () =>
        {
            await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async (sp, context) =>
            {
                var sqlFile = configuration["sql-file"];
                var sourceSchema = configuration["source-schema"];
                ValidateRequired(sqlFile, "--sql-file");
                ValidateRequired(sourceSchema, "--source-schema");

                var runner = new SqlScriptRunner(context);
                await runner.ExecuteFileAsync(sqlFile!, new Dictionary<string, string?>
                {
                    ["{SOURCE_PROFITSHARE_SCHEMA}"] = sourceSchema
                }, sourceSchema);

                await GatherSchemaStatistics(context);

                // Resolve and run the rebuild service before gathering schema statistics
                var rebuildService = sp.GetRequiredService<RebuildEnrollmentAndZeroContService>();
                await rebuildService.ExecuteAsync(CancellationToken.None);
            });
        });

        var runSqlCommandForNavigation = new Command("import-from-navigation", "Run a custom SQL script to add all navigations");
        commonOptions.ForEach(runSqlCommandForNavigation.AddOption);

        runSqlCommandForNavigation.SetHandler(async () =>
        {
            await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async (_, context) =>
            {
                var sqlFile = configuration["sql-file"];
                var sourceSchema = configuration["source-schema"];
                ValidateRequired(sqlFile, "--sql-file");
                ValidateRequired(sourceSchema, "--source-schema");

                var runner = new SqlScriptRunner(context);
                await runner.ExecuteFileAsync(sqlFile!, new Dictionary<string, string?>
                {
                    ["{SOURCE_PROFITSHARE_SCHEMA}"] = sourceSchema
                }, sourceSchema);
            });
        });

        var generateDgmlCommand = new Command("generate-dgml", "Generate a DGML file for the DbContext model");
        commonOptions.ForEach(generateDgmlCommand.AddOption);

        generateDgmlCommand.SetHandler(async () =>
        {
            await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async (_, context) =>
            {
                var outputFile = configuration["output-file"];
                if (string.IsNullOrEmpty(outputFile))
                {
                    throw new ArgumentNullException(nameof(outputFile), "Output file path must be provided.");
                }

                var dgml = context.AsDgml();
                await File.WriteAllTextAsync(outputFile, dgml, Encoding.UTF8);
                Console.WriteLine($"DGML file created: {outputFile}");
            });
        });

        var generateMarkdownCommand = new Command("generate-markdown", "Generate a Markdown file from DGML");
        commonOptions.ForEach(generateMarkdownCommand.AddOption);

        generateMarkdownCommand.SetHandler(async () =>
        {
            var outputFile = configuration["output-file"];
            if (string.IsNullOrEmpty(outputFile))
            {
                throw new ArgumentNullException(nameof(outputFile), "Output file path must be provided.");
            }

            await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async (_, context) =>
            {
                var dgml = context.AsDgml();
                await DgmlService.GenerateMarkdownFromDgml(dgml, outputFile);
                Console.WriteLine($"Markdown file created: {outputFile}");
            });
        });

        var validateImportCommand = new Command("validate-import", "validate an import from ready against an existing database");
        commonOptions.ForEach(validateImportCommand.AddOption);
        validateImportCommand.Add(new Option<string>("--current-year", "Current year of profit sharing"));

        validateImportCommand.SetHandler(async iCtx =>
        {
            await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async (_, context) =>
            {
                try
                {
                    var sqlFile = configuration["sql-file"];
                    var sourceSchema = configuration["source-schema"];
                    if (string.IsNullOrEmpty(sqlFile) || string.IsNullOrEmpty(sourceSchema))
                    {
                        throw new ArgumentNullException("SQL file path and schema must be provided.");
                    }

                    string sqlCommand = await File.ReadAllTextAsync(sqlFile);
                    sqlCommand = sqlCommand.Replace("COMMIT ;", string.Empty)
                        .Replace("{SOURCE_PROFITSHARE_SCHEMA}", sourceSchema)
                        .Replace("{CURRENT_YEAR}", configuration["current-year"]).Trim();

                    await context.Database.ExecuteSqlRawAsync(sqlCommand);
                }
                catch (Exception e)
                {
                    iCtx.Console.WriteLine(e.ToString());
                    iCtx.ExitCode = 1;
                }
            });
        });

        rootCommand.AddCommand(upgradeDbCommand);
        rootCommand.AddCommand(dropRecreateDbCommand);
        rootCommand.AddCommand(runSqlCommand);
        rootCommand.AddCommand(generateDgmlCommand);
        rootCommand.AddCommand(generateMarkdownCommand);
        rootCommand.AddCommand(GenerateScriptHelper.CreateGenerateUpgradeScriptCommand(configuration, args, commonOptions));
        rootCommand.AddCommand(validateImportCommand);
        rootCommand.AddCommand(runSqlCommandForNavigation);

        var runSqlCommandForUatNavigation = new Command("import-uat-navigation", "Run a custom SQL script to add UAT navigations");
        commonOptions.ForEach(runSqlCommandForUatNavigation.AddOption);

        runSqlCommandForUatNavigation.SetHandler(async () =>
        {
            await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async (_, context) =>
            {
                var sqlFile = configuration["sql-file"];
                var sourceSchema = configuration["source-schema"];
                ValidateRequired(sqlFile, "--sql-file");
                ValidateRequired(sourceSchema, "--source-schema");

                var runner = new SqlScriptRunner(context);
                await runner.ExecuteFileAsync(sqlFile!, new Dictionary<string, string?>
                {
                    ["{SOURCE_PROFITSHARE_SCHEMA}"] = sourceSchema
                }, sourceSchema);
            });
        });

        rootCommand.AddCommand(runSqlCommandForUatNavigation);

        return rootCommand.InvokeAsync(args);
    }

    private static async Task GatherSchemaStatistics(ProfitSharingDbContext context)
    {
        OracleConnectionStringBuilder sb = new OracleConnectionStringBuilder(context.Database.GetConnectionString());

        string gatherStats = $@"BEGIN
   DBMS_STATS.GATHER_SCHEMA_STATS('{sb.UserID}');
    END;";

        await context.Database.ExecuteSqlRawAsync(gatherStats);
        Console.WriteLine("Gathered schema stats");
    }

    private static void ValidateRequired(string? value, string optionName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(optionName, $"{optionName} must be provided.");
        }
    }
}
