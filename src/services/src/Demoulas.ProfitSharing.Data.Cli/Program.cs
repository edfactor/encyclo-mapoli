using System.CommandLine;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Data.Cli.DiagramEntities;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            await ExecuteWithDbContext(configuration, args, async context => { await context.Database.MigrateAsync(); });
        });

        var dropRecreateDbCommand = new Command("drop-recreate-db", "Drop and recreate the database");
        commonOptions.ForEach(dropRecreateDbCommand.AddOption);

        dropRecreateDbCommand.SetHandler(async () =>
        {
            await ExecuteWithDbContext(configuration, args, async context =>
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
            });
        });

        var runSqlCommand = new Command("import-from-ready", "Run a custom SQL script after migrations");
        commonOptions.ForEach(runSqlCommand.AddOption);

        runSqlCommand.SetHandler(async () =>
        {
            await ExecuteWithDbContext(configuration, args, async context =>
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
            });
        });

        var generateDgmlCommand = new Command("generate-dgml", "Generate a DGML file for the DbContext model");
        commonOptions.ForEach(generateDgmlCommand.AddOption);

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

    private static async Task GenerateMarkdownFromDgml(string dgmlContent, string outputFile)
    {
        // Parse DGML content
        //XmlSerializer is treating "False"(with an uppercase "F") as invalid.
        var xmlRoot = new XmlRootAttribute
        {
            ElementName = "DirectedGraph",
            Namespace = "http://schemas.microsoft.com/vs/2009/dgml"
        };
        XmlSerializer serializer = new XmlSerializer(typeof(DirectedGraph), xmlRoot);
        using StringReader stringReader = new StringReader(dgmlContent.Replace("True", "true").Replace("False", "false"));
        using XmlReader xmlReader = XmlReader.Create(stringReader);
        var directedGraph = serializer.Deserialize(xmlReader) as DirectedGraph;

        if (directedGraph == null || directedGraph.Nodes == null || directedGraph.Links == null)
        {
            throw new InvalidOperationException("Invalid or empty DGML content.");
        }

        // Process Nodes: Group by Id to handle duplicates
        var tables = directedGraph.Nodes.Node
            .Where(node => node.Category == "EntityType" && !string.IsNullOrEmpty(node.Id))
            .GroupBy(node => node.Id!)
            .ToDictionary(
                group => group.Key,
                group => group.First().Label ?? group.Key);

        var columns = directedGraph.Nodes.Node
            .Where(node => node.Category?.Contains("Property") == true && !string.IsNullOrEmpty(node.Id))
            .GroupBy(node => node.Id!)
            .ToDictionary(
                group => group.Key,
                group => new
                {
                    EntityPropertyName = group.First().Label ?? group.Key,
                    DataType = group.First().Type ?? "N/A",
                    Precision = group.First().MaxLength ?? "N/A",
                    Explanation = group.First().Annotations ?? "N/A",
                    IsPrimaryKey = group.First().IsPrimaryKey,
                    IsForeignKey = group.First().IsForeignKey,
                    IsIndexed = group.First().IsIndexed,
                    IsRequired = group.First().IsRequired,
                    ColumnName = ExtractColumnName(group.First().Annotations) ?? group.First().Category ?? group.Key,
                });

        // Process Links: Map table IDs to column IDs
        var tableColumnsMap = directedGraph.Links.Link
            .Where(link => !string.IsNullOrEmpty(link.Source) && !string.IsNullOrEmpty(link.Target))
            .GroupBy(link => link.Source!)
            .ToDictionary(
                group => group.Key,
                group => group.Select(link => link.Target!).ToList());

        // Build Markdown content
        var markdown = new StringBuilder();
        markdown.AppendLine("# Database Schema Representation");

        foreach (var table in tables)
        {
            markdown.AppendLine($"\n## Table: **{table.Value}**\n");
            markdown.AppendLine("| Entity Name | Column Name | Data Type | Precision | IsPrimaryKey | IsForeignKey | IsRequired | IsIndexed |");
            markdown.AppendLine("|-------------|-------------|-----------|-----------|--------------|--------------|------------|-----------|");

            if (tableColumnsMap.TryGetValue(table.Key, out var columnIds))
            {
                foreach (var columnId in columnIds.Distinct())
                {
                    if (columns.TryGetValue(columnId, out var column))
                    {
                        markdown.AppendLine($"| {column.EntityPropertyName} | {column.ColumnName} | {column.DataType} | {column.Precision} | {column.IsPrimaryKey} | {column.IsForeignKey} | {column.IsRequired} | {column.IsIndexed} |");
                    }
                }
            }
            else
            {
                markdown.AppendLine("| No columns found | N/A | N/A | N/A |");
            }
        }

        // Save to output file
        await File.WriteAllTextAsync(outputFile, markdown.ToString());
    }

    private static string? ExtractColumnName(string? annotations)
    {
        if (string.IsNullOrEmpty(annotations))
        {
            return null;
        }

        var match = System.Text.RegularExpressions.Regex.Match(annotations, @"Relational:ColumnName:\s*(\S+)");
        return match.Success ? match.Groups[1].Value : null;
    }


    private static HostApplicationBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddUserSecrets<Program>();
        builder.Configuration.AddEnvironmentVariables();
        return builder;
    }
}
