using System.CommandLine;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Factories;
using Microsoft.Extensions.DependencyInjection;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

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
            .Build();

        var rootCommand = new RootCommand("CLI tool for database operations");

        var upgradeDbCommand = new Command("upgrade-db", "Apply migrations to upgrade the database")
        {
            new Option<string>("--connection-name", "The name of the configuration property that holds the connection string")
        };

        upgradeDbCommand.SetHandler(async () =>
        {
            await ExecuteWithDbContext(configuration, args, async context =>
            {
                await context.Database.MigrateAsync();
            });
        });

        var dropRecreateDbCommand = new Command("drop-recreate-db", "Drop and recreate the database")
        {
            new Option<string>("--connection-name", "The name of the configuration property that holds the connection string")
        };

        dropRecreateDbCommand.SetHandler(async () =>
        {
            await ExecuteWithDbContext(configuration, args, async context =>
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
            });
        });

        var runSqlCommand = new Command("import-from-ready", "Run a custom SQL script after migrations")
        {
            new Option<string>("--connection-name", "The name of the configuration property that holds the connection string"),
            new Option<string>("--sql-file", "The path to the custom SQL file"),
            new Option<string>("--source-Schema", "Name of the schema that is being used as the source database")
        };

        runSqlCommand.SetHandler(async () =>
        {
            await ExecuteWithDbContext(configuration, args, async context =>
            {
                var sqlFile = configuration["sql-file"];
                var sourceSchema = configuration["source-Schema"];
                if (string.IsNullOrEmpty(sqlFile) || string.IsNullOrEmpty(sourceSchema))
                {
                    throw new ArgumentNullException("SQL file path and schema must be provided.");
                }

                string sqlCommand = await File.ReadAllTextAsync(sqlFile);
                sqlCommand = sqlCommand.Replace("COMMIT ;", string.Empty)
                    .Replace("{SOURCE_PROFITSHARE_SCHEMA}", sourceSchema).Trim();
                await context.Database.ExecuteSqlRawAsync(sqlCommand);
            });
        });

        var generateDgmlCommand = new Command("generate-dgml", "Generate a DGML file for the DbContext model")
        {
            new Option<string>("--connection-name", "The name of the configuration property that holds the connection string"),
            new Option<string>("--output-file", "The path to save the DGML file")
        };

        generateDgmlCommand.SetHandler(async () =>
        {
            await ExecuteWithDbContext(configuration, args, async context =>
            {
                var outputFile = configuration["output-file"];
                if (string.IsNullOrEmpty(outputFile))
                {
                    throw new ArgumentNullException(nameof(outputFile), "Output file path must be provided.");
                }

                var dgml = context.AsDgml();
                await File.WriteAllTextAsync(outputFile, dgml, System.Text.Encoding.UTF8);
                Console.WriteLine($"DGML file created: {outputFile}");
            });
        });

        var generateMarkdownCommand = new Command("generate-markdown", "Generate a Markdown file from DGML")
        {
            new Option<string>("--connection-name", "The name of the configuration property that holds the connection string"),
            new Option<string>("--output-file", "The path to save the Markdown file")
        };

        generateMarkdownCommand.SetHandler(async () =>
            {
                var outputFile = configuration["output-file"];
                if (string.IsNullOrEmpty(outputFile))
                {
                    throw new ArgumentNullException(nameof(outputFile), "Output file path must be provided.");
                }

                await ExecuteWithDbContext(configuration, args, async context =>
                {
                    var dgml = context.AsDgml();
                    await GenerateMarkdownFromDgml(dgml, outputFile);
                    Console.WriteLine($"Markdown file created: {outputFile}");
                });
            });


        rootCommand.AddCommand(upgradeDbCommand);
        rootCommand.AddCommand(dropRecreateDbCommand);
        rootCommand.AddCommand(runSqlCommand);
        rootCommand.AddCommand(generateDgmlCommand);
        rootCommand.AddCommand(generateMarkdownCommand);

        return rootCommand.InvokeAsync(args);
    }

    private static async Task ExecuteWithDbContext(IConfiguration configuration, string[] args, Func<ProfitSharingDbContext, Task> action)
    {
        string? connectionName = configuration["connection-name"];
        if (string.IsNullOrEmpty(connectionName))
        {
            throw new ArgumentNullException(nameof(connectionName), "Connection name must be provided.");
        }

        HostApplicationBuilder builder = CreateHostBuilder(args);
        var list = new List<ContextFactoryRequest> { ContextFactoryRequest.Initialize<ProfitSharingDbContext>(connectionName) };
        _ = DataContextFactory.Initialize(builder, contextFactoryRequests: list);

        await using var context = builder.Services.BuildServiceProvider().GetRequiredService<ProfitSharingDbContext>();
        await action(context);
#pragma warning restore S3928
    }

    private static Task GenerateMarkdownFromDgml(string dgmlContent, string outputFile)
    {
        // Parse DGML content
        var dgml = XDocument.Parse(dgmlContent);
        XNamespace ns = "http://schemas.microsoft.com/vs/2009/dgml";

        // Parse Tables: Group by Id to handle duplicates
        var tables = dgml.Descendants(ns + "Node")
            .Where(node => node.Attribute("Category")?.Value == "EntityType" && node.Attribute("Id") != null)
            .GroupBy(node => node.Attribute("Id")!.Value) // Group by Id
            .ToDictionary(
                group => group.Key,
                group => group.First().Attribute("Label")?.Value ?? group.Key);

        // Parse Columns: Group by Id to handle duplicates
        var columns = dgml.Descendants(ns + "Node")
            .Where(node => node.Attribute("Category")?.Value?.Contains("Property") == true && node.Attribute("Id") != null)
            .GroupBy(node => node.Attribute("Id")!.Value) // Group by Id
            .ToDictionary(
                group => group.Key,
                group => new
                {
                    ColumnName = group.First().Attribute("Label")?.Value ?? group.Key,
                    DataType = group.First().Attribute("DataType")?.Value ?? "N/A",
                    Precision = group.First().Attribute("Precision")?.Value ?? "N/A",
                    Explanation = group.First().Attribute("Annotations")?.Value ?? "N/A"
                });

        // Parse Links to associate Columns with Tables
        var tableColumnsMap = dgml.Descendants(ns + "Link")
            .Where(link => link.Attribute("Source") != null && link.Attribute("Target") != null)
            .GroupBy(link => link.Attribute("Source")!.Value) // Group links by Source (table)
            .ToDictionary(
                group => group.Key,
                group => group.Select(link => link.Attribute("Target")!.Value).ToList());

        // Build Markdown content
        var markdown = new System.Text.StringBuilder();
        markdown.AppendLine("# Database Schema Representation");

        foreach (var table in tables)
        {
            markdown.AppendLine($"\n## Table: **{table.Value}**\n");
            markdown.AppendLine("| Column Name | Data Type | Precision | Explanation |");
            markdown.AppendLine("|-------------|-----------|-----------|-------------|");

            if (tableColumnsMap.TryGetValue(table.Key, out var columnIds))
            {
                foreach (var columnId in columnIds.Distinct()) // Ensure unique column references
                {
                    if (columns.TryGetValue(columnId, out var column))
                    {
                        markdown.AppendLine($"| {column.ColumnName} | {column.DataType} | {column.Precision} | {column.Explanation} |");
                    }
                }
            }
            else
            {
                markdown.AppendLine("| No columns found | N/A | N/A | N/A |");
            }
        }

        // Save to output file
        return File.WriteAllTextAsync(outputFile, markdown.ToString());
    }




    private static HostApplicationBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddUserSecrets<Program>();
        builder.Configuration.AddEnvironmentVariables();
        return builder;
    }
}
