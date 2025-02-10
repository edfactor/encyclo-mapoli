using System.CommandLine;
using System.Text;
using Demoulas.ProfitSharing.Data.Cli.DiagramServices;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

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
            await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async context => { await context.Database.MigrateAsync(); });
        });

        var dropRecreateDbCommand = new Command("drop-recreate-db", "Drop and recreate the database");
        commonOptions.ForEach(dropRecreateDbCommand.AddOption);

        dropRecreateDbCommand.SetHandler(async () =>
        {
            await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async context =>
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
            });
        });

        var runSqlCommand = new Command("import-from-ready", "Run a custom SQL script after migrations");
        commonOptions.ForEach(runSqlCommand.AddOption);

        runSqlCommand.SetHandler(async () =>
        {
            await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async context =>
            {
                var sqlFile = configuration["sql-file"];
                var sourceSchema = configuration["source-schema"];
                if (string.IsNullOrEmpty(sqlFile) || string.IsNullOrEmpty(sourceSchema))
                {
                    throw new ArgumentNullException("SQL file path and schema must be provided.");
                }

                string sqlCommand = await File.ReadAllTextAsync(sqlFile);
                sqlCommand = sqlCommand.Replace("COMMIT ;", string.Empty)
                    .Replace("{SOURCE_PROFITSHARE_SCHEMA}", sourceSchema).Trim();
                await context.Database.ExecuteSqlRawAsync(sqlCommand);

                context.DataImportRecords.Add(new DataImportRecord { SourceSchema = sourceSchema });
                await context.SaveChangesAsync();
            });
        });

        var generateDgmlCommand = new Command("generate-dgml", "Generate a DGML file for the DbContext model");
        commonOptions.ForEach(generateDgmlCommand.AddOption);

        generateDgmlCommand.SetHandler(async () =>
        {
            await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async context =>
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

            await GenerateScriptHelper.ExecuteWithDbContext(configuration, args, async context =>
            {
                var dgml = context.AsDgml();
                await DgmlService.GenerateMarkdownFromDgml(dgml, outputFile);
                Console.WriteLine($"Markdown file created: {outputFile}");
            });
        });


        rootCommand.AddCommand(upgradeDbCommand);
        rootCommand.AddCommand(dropRecreateDbCommand);
        rootCommand.AddCommand(runSqlCommand);
        rootCommand.AddCommand(generateDgmlCommand);
        rootCommand.AddCommand(generateMarkdownCommand);
        rootCommand.AddCommand(GenerateScriptHelper.CreateGenerateUpgradeScriptCommand(configuration, args, commonOptions));

        return rootCommand.InvokeAsync(args);
    }
}
