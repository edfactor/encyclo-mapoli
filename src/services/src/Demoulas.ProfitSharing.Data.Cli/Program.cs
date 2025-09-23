using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Text;
using Demoulas.Common.Contracts.Configuration;
using Demoulas.Common.Data.Services.Entities.Contexts.EntityMapping.Data;
using Demoulas.ProfitSharing.Data.Cli.DiagramServices;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Services.LogMasking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
        commonOptions.ForEach(o => upgradeDbCommand.Add(o));

        // Handler implemented in dispatcher below

        var dropRecreateDbCommand = new Command("drop-recreate-db", "Drop and recreate the database");
        commonOptions.ForEach(o => dropRecreateDbCommand.Add(o));

        // Handler implemented in dispatcher below

        var runSqlCommand = new Command("import-from-ready", "Run a custom SQL script after migrations");
        commonOptions.ForEach(o => runSqlCommand.Add(o));

        // Handler implemented in dispatcher below

        var runSqlCommandForNavigation = new Command("import-from-navigation", "Run a custom SQL script to add all navigations");
        commonOptions.ForEach(o => runSqlCommandForNavigation.Add(o));

        // Handler implemented in dispatcher below

        var generateDgmlCommand = new Command("generate-dgml", "Generate a DGML file for the DbContext model");
        commonOptions.ForEach(o => generateDgmlCommand.Add(o));

        // Handler implemented in dispatcher below

        var generateMarkdownCommand = new Command("generate-markdown", "Generate a Markdown file from DGML");
        commonOptions.ForEach(o => generateMarkdownCommand.Add(o));

        // Handler implemented in dispatcher below

        var validateImportCommand = new Command("validate-import", "validate an import from ready against an existing database");
        commonOptions.ForEach(o => validateImportCommand.Add(o));
        validateImportCommand.Add(new Option<string>("--current-year", "Current year of profit sharing"));

        // Handler implemented in dispatcher below

        rootCommand.Add(upgradeDbCommand);
        rootCommand.Add(dropRecreateDbCommand);
        rootCommand.Add(runSqlCommand);
        rootCommand.Add(generateDgmlCommand);
        rootCommand.Add(generateMarkdownCommand);
        rootCommand.Add(GenerateScriptHelper.CreateGenerateUpgradeScriptCommand(configuration, args, commonOptions));
        rootCommand.Add(validateImportCommand);
        rootCommand.Add(runSqlCommandForNavigation);

        var runSqlCommandForUatNavigation = new Command("import-uat-navigation", "Run a custom SQL script to add UAT navigations");
        commonOptions.ForEach(o => runSqlCommandForUatNavigation.Add(o));

        rootCommand.Add(runSqlCommandForUatNavigation);

        // Manual dispatch to avoid depending on SetHandler/Handler extensions of System.CommandLine
        // Map command name (first argument) to the logic above.
        string? cmd = args.Length > 0 ? args[0] : null;

        return cmd switch
        {
            "upgrade-db" => ExecuteUpgradeDb(configuration, args),
            "drop-recreate-db" => ExecuteDropRecreateDb(configuration, args),
            "import-from-ready" => ExecuteImportFromReady(configuration, args),
            "import-from-navigation" => ExecuteImportFromNavigation(configuration, args),
            "generate-dgml" => ExecuteGenerateDgml(configuration, args),
            "generate-markdown" => ExecuteGenerateMarkdown(configuration, args),
            "validate-import" => ExecuteValidateImport(configuration, args),
            "import-uat-navigation" => ExecuteImportFromUatNavigation(configuration, args),
            "generate-upgrade-script" => ExecuteGenerateUpgradeScript(configuration, args),
            _ => RootCommandFallback(rootCommand, args)
        };
    }

    // Dispatcher implementations
    private static Task<int> RootCommandFallback(RootCommand root, string[] args)
    {
        _ = args;
        // Fallback to showing help when no command matches
        Console.WriteLine(root.Description);
        Console.WriteLine("Available commands:");
        foreach (var c in root.Children.OfType<Command>())
        {
            Console.WriteLine($"  {c.Name} - {c.Description}");
        }
        return Task.FromResult(1);
    }

    private static async Task<int> ExecuteUpgradeDb(IConfiguration configuration, string[] args)
    {
        _ = args;
        await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async (_, context) =>
        {
            await context.Database.MigrateAsync();

            var existingIds = await context.AccountingPeriods.Select(p => p.WeekendingDate).ToListAsync();
            var newRecords = CaldarRecordSeeder.Records.Where(p => !existingIds.Contains(p.WeekendingDate)).ToList();
            if (newRecords.Any())
            {
                context.AccountingPeriods.AddRange(newRecords);
                await context.SaveChangesAsync();
            }
            await GatherSchemaStatistics(context);
        });
        return 0;
    }

    private static async Task<int> ExecuteDropRecreateDb(IConfiguration configuration, string[] args)
    {
        _ = args;
        await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async (_, context) =>
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();
        });
        return 0;
    }

    private static async Task<int> ExecuteImportFromReady(IConfiguration configuration, string[] args)
    {
        _ = args;
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

            var rebuildService = sp.GetRequiredService<RebuildEnrollmentAndZeroContService>();
            await rebuildService.ExecuteAsync(CancellationToken.None);
        });
        return 0;
    }

    private static async Task<int> ExecuteImportFromNavigation(IConfiguration configuration, string[] args)
    {
        _ = args;
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
        return 0;
    }

    private static Task<int> ExecuteImportFromUatNavigation(IConfiguration configuration, string[] args)
    {
        _ = args;
        // Reuse the same implementation as non-UAT navigation import to avoid duplicate code
        return ExecuteImportFromNavigation(configuration, args);
    }

    private static async Task<int> ExecuteGenerateDgml(IConfiguration configuration, string[] args)
    {
        _ = args; // Marking args to satisfy analyzer about unused parameter 'args'
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
        return 0;
    }

    private static async Task<int> ExecuteGenerateMarkdown(IConfiguration configuration, string[] args)
    {
        _ = args;
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
        return 0;
    }

    private static async Task<int> ExecuteValidateImport(IConfiguration configuration, string[] args)
    {
        _ = args;
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
                Console.WriteLine(e.ToString());
            }
        });
        return 0;
    }

    private static async Task<int> ExecuteGenerateUpgradeScript(IConfiguration configuration, string[] args)
    {
        _ = args;
        var cmd = GenerateScriptHelper.CreateGenerateUpgradeScriptCommand(configuration, args, new List<Option>
        {
            new Option<string>("--output-file")
        });

        // Execute the logic directly
        await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async (sp, dbContext) =>
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

            var outputPath = configuration["output-file"];
            var script = migrator.GenerateScript(
                fromMigration: currentMigration,
                toMigration: null,
                options: MigrationsSqlGenerationOptions.Idempotent | MigrationsSqlGenerationOptions.Script
            );

            if (!string.IsNullOrWhiteSpace(outputPath))
            {
                var fullPath = Path.GetFullPath(outputPath);
                var directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(fullPath, script);
                Console.WriteLine($"SQL upgrade script generated at: {fullPath}");
            }
            else
            {
                Console.WriteLine("Generated SQL Script:");
                Console.WriteLine(script);
            }
        });

        return 0;
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
