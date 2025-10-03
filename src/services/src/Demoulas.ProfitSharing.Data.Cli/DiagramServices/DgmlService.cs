using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Demoulas.ProfitSharing.Data.Cli.DiagramEntities;

namespace Demoulas.ProfitSharing.Data.Cli.DiagramServices;

internal static class DgmlService
{
    /// <summary>
    /// Generates a Markdown representation of a database schema from the provided DGML content.
    /// </summary>
    /// <param name="dgmlContent">
    /// The DGML content as a string. This should represent a directed graph in the DGML format.
    /// </param>
    /// <param name="outputFile">
    /// The path to the output file where the generated Markdown content will be saved.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation of generating and saving the Markdown file.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the provided DGML content is invalid or empty.
    /// </exception>
    public static async Task GenerateMarkdownFromDgml(string dgmlContent, string outputFile)
    {
        // Parse DGML content
        //XmlSerializer is treating "False"(with an uppercase "F") as invalid.
        var xmlRoot = new XmlRootAttribute { ElementName = "DirectedGraph", Namespace = "http://schemas.microsoft.com/vs/2009/dgml" };
        XmlSerializer serializer = new XmlSerializer(typeof(DirectedGraph), xmlRoot);
        using StringReader stringReader = new StringReader(dgmlContent.Replace("True", "true").Replace("False", "false"));
        using XmlReader xmlReader = XmlReader.Create(stringReader);
        var directedGraph = serializer.Deserialize(xmlReader) as DirectedGraph;

        if (directedGraph?.Nodes == null || directedGraph.Links == null)
        {
            throw new InvalidOperationException("Invalid or empty DGML content.");
        }

        const string navProperty = "Navigation Property";

        // Process Links: Map PKs and FKs
        var relationships = directedGraph.Links.Link
            .Where(link => !string.IsNullOrEmpty(link.Source) && !string.IsNullOrEmpty(link.Target))
            .Select(link => new
            {
                Source = link.Source!,
                Target = link.Target!,
                RelationshipType = ExtractRelationshipType(link), // Determine FK/PK/Other
                ParsedAnnotations = ParseAnnotations(link.Annotations ?? ""), // Parse annotations
            })
            .GroupBy(link => link.Source)
            .ToDictionary(
                group => group.Key,
                group => group.ToList()
            );



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
                    EntityPropertyName = group.First().Name ?? group.Key,
                    DataType = group.First().Category == navProperty ? group.First().Category : group.First().Type ?? "N/A",
                    Precision = ExtractPrecision(group.First()),
                    Explanation = group.First().Annotations ?? "N/A",
                    IsPrimaryKey = group.First().IsPrimaryKey,
                    IsForeignKey = group.First().IsForeignKey,
                    IsIndexed = group.First().IsIndexed,
                    IndexName = ExtractIndexName(group.First()),
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

            var primaryKeys = new List<string>();
            var foreignKeys = new List<string>();
            var fkRelationships = new List<string>();

            if (tableColumnsMap.TryGetValue(table.Key, out var columnIds))
            {
                foreach (var columnId in columnIds.Distinct())
                {
                    if (columns.TryGetValue(columnId, out var column))
                    {
                        // Add to PK, FK, and Index lists for separate summary
                        if (column.IsPrimaryKey) { primaryKeys.Add(column.ColumnName); }
                        if (column.IsForeignKey) { foreignKeys.Add(column.ColumnName); }

                        markdown.AppendLine(
                            $"| {column.EntityPropertyName} | {column.ColumnName} | {column.DataType} | {column.Precision} | {column.IsPrimaryKey} | {column.IsForeignKey} | {column.IsRequired} | {column.IsIndexed} |");
                    }
                }
            }
            else
            {
                markdown.AppendLine("| No columns found | N/A | N/A | N/A | N/A | N/A | N/A | N/A |");
            }

            // Add FK Relationships
            if (relationships.TryGetValue(table.Key, out var tableRelationships))
            {
                foreach (var rel in tableRelationships)
                {
                    if (rel.ParsedAnnotations.TryGetValue("RelationalName", out var fkName))
                    {
                        fkRelationships.Add($"{rel.Target} (FK Name: {fkName})");
                    }
                }
            }

            // Append PK, FK, and Index summaries
            markdown.AppendLine("\n### Summary");
            markdown.AppendLine($"- **Primary Keys**: {(primaryKeys.Any() ? string.Join(", ", primaryKeys) : "None")}");
            markdown.AppendLine($"- **Foreign Keys**: {(foreignKeys.Any() ? string.Join(", ", foreignKeys) : "None")}");

            if (fkRelationships.Any())
            {
                markdown.AppendLine("- **Foreign Key Relationships**:");
                foreach (var fkRelationship in fkRelationships)
                {
                    markdown.AppendLine($"  - {fkRelationship}");
                }
            }
            else
            {
                markdown.AppendLine("- **Foreign Key Relationships**: None");
            }

        }

        // Save to output file
        await File.WriteAllTextAsync(outputFile, markdown.ToString());
    }

    internal static string ExtractColumnName(Node node)
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

    internal static string ExtractIndexName(Node node)
    {
        if (!string.IsNullOrWhiteSpace(node.Annotations))
        {
            var match = System.Text.RegularExpressions.Regex.Match(node.Annotations, @"IndexName:\s*(\S+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
        }

        return "Unnamed Index"; // Fallback for indexes without names
    }

    internal static string ExtractPrecision(Node node)
    {
        if (!string.IsNullOrWhiteSpace(node.MaxLength) && node.MaxLength != "None")
        {
            return node.MaxLength;
        }

        if (!string.IsNullOrWhiteSpace(node.Annotations))
        {
            var match = System.Text.RegularExpressions.Regex.Match(node.Annotations, @"Precision:\s*(\d+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
        }

        return "None"; // Fallback if precision is not found
    }
    internal static string ExtractRelationshipType(Link link)
    {
        if (!string.IsNullOrWhiteSpace(link.Annotations))
        {
            if (link.Annotations.Contains("ForeignKey"))
            {
                return "ForeignKey";
            }

            if (link.Annotations.Contains("PrimaryKey"))
            {
                return "PrimaryKey";
            }
        }

        return "Other";
    }

    internal static Dictionary<string, string> ParseAnnotations(string annotations)
    {
        var result = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(annotations))
        {
            // Regex for "Relational:Name: <Value>"
            var match = System.Text.RegularExpressions.Regex.Match(annotations, @"Relational:Name:\s*(\S+)");
            if (match.Success)
            {
                result["RelationalName"] = match.Groups[1].Value;
            }

            // Add additional regex patterns here for other types of annotations if needed
        }

        return result;
    }


}
