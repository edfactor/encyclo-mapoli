using System.Text;
using Demoulas.ProfitSharing.Data.Cli.DiagramEntities;
using System.Xml.Serialization;
using System.Xml;

namespace Demoulas.ProfitSharing.Data.Cli.DiagramServices;

internal static class DgmlService
{
    public static async Task GenerateMarkdownFromDgml(string dgmlContent, string outputFile)
    {
        // Parse DGML content
        //XmlSerializer is treating "False"(with an uppercase "F") as invalid.
        var xmlRoot = new XmlRootAttribute { ElementName = "DirectedGraph", Namespace = "http://schemas.microsoft.com/vs/2009/dgml" };
        XmlSerializer serializer = new XmlSerializer(typeof(DirectedGraph), xmlRoot);
        using StringReader stringReader = new StringReader(dgmlContent.Replace("True", "true").Replace("False", "false"));
        using XmlReader xmlReader = XmlReader.Create(stringReader);
        var directedGraph = serializer.Deserialize(xmlReader) as DirectedGraph;

        if (directedGraph == null || directedGraph.Nodes == null || directedGraph.Links == null)
        {
            throw new InvalidOperationException("Invalid or empty DGML content.");
        }

        const string navProperty = "Navigation Property";

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
                    DataType =  group.First().Category == navProperty ? group.First().Category : group.First().Type ?? "N/A",
                    Precision = group.First().MaxLength ?? "N/A",
                    Explanation = group.First().Annotations ?? "N/A",
                    IsPrimaryKey = group.First().IsPrimaryKey,
                    IsForeignKey = group.First().IsForeignKey,
                    IsIndexed = group.First().IsIndexed,
                    IsRequired = group.First().IsRequired,
                    ColumnName = ExtractColumnName(group.First()),
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
                        markdown.AppendLine(
                            $"| {column.EntityPropertyName} | {column.ColumnName} | {column.DataType} | {column.Precision} | {column.IsPrimaryKey} | {column.IsForeignKey} | {column.IsRequired} | {column.IsIndexed} |");
                    }
                }
            }
            else
            {
                markdown.AppendLine("| No columns found | N/A | N/A | N/A | N/A | N/A | N/A | N/A |");
            }
        }

        // Save to output file
        await File.WriteAllTextAsync(outputFile, markdown.ToString());
    }

    public static string ExtractColumnName(Node node)
    {
        if (!string.IsNullOrWhiteSpace(node.Annotations))
        {
            var match = System.Text.RegularExpressions.Regex.Match(node.Annotations, @"Relational:ColumnName:\s*(\S+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
        }

        return $"{node.Id}";
    }
}
