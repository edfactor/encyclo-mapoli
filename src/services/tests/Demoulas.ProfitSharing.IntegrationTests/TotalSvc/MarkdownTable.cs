using System.Text;

namespace Demoulas.ProfitSharing.IntegrationTests.TotalSvc;

// Simple code for emitting a markdown table

public class MarkdownTable
{
    private readonly List<string> _headers;
    private readonly List<(string[] Columns, string? CssClass)> _rows;

    public MarkdownTable(string[] headers)
    {
        _headers = headers.ToList();
        _rows = new List<(string[], string?)>();
    }

    public void AddRow(params string[] columns)
    {
        AddRow(null, columns);
    }

    public void AddRow(string? cssClass, params string[] columns)
    {
        if (columns.Length != _headers.Count)
        {
            throw new ArgumentException($"Expected {_headers.Count} columns, but got {columns.Length}.");
        }

        _rows.Add((columns, cssClass));
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        // Calculate the maximum width for each column
        int[] columnWidths = CalculateColumnWidths();

        // Add the header row with proper padding
        sb.AppendLine("| " + string.Join(" | ", _headers.Select((header, i) => header.PadRight(columnWidths[i]))) + " |");

        // Add the separator row with proper width
        sb.AppendLine("| " + string.Join(" | ", columnWidths.Select(width => new string('-', width))) + " |");

        // Add data rows with proper padding
        foreach ((string[] row, string? _) in _rows)
        {
            sb.AppendLine("| " + string.Join(" | ", row.Select((cell, i) => cell.PadRight(columnWidths[i]))) + " |");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Calculates the maximum width needed for each column by scanning headers and all row data.
    /// </summary>
    private int[] CalculateColumnWidths()
    {
        int[] widths = new int[_headers.Count];

        // Initialize with header widths
        for (int i = 0; i < _headers.Count; i++)
        {
            widths[i] = _headers[i].Length;
        }

        // Update with maximum cell width from each row
        foreach ((string[] row, string? _) in _rows)
        {
            for (int i = 0; i < row.Length && i < widths.Length; i++)
            {
                widths[i] = Math.Max(widths[i], row[i]?.Length ?? 0);
            }
        }

        return widths;
    }

    public List<string[]> rows()
    {
        return _rows.Select(r => r.Columns).ToList();
    }

    public string ToHtml(string title = "Total Service Discrepancies", Dictionary<string, string>? summaryStats = null)
    {
        StringBuilder sb = new();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset=\"UTF-8\">");
        sb.AppendLine($"    <title>{System.Net.WebUtility.HtmlEncode(title)}</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; background-color: #f5f5f5; }");
        sb.AppendLine("        table { border-collapse: collapse; width: 100%; background-color: white; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
        sb.AppendLine("        th { background-color: #2c3e50; color: white; padding: 12px; text-align: left; font-weight: 600; }");
        sb.AppendLine("        td { padding: 10px; border-bottom: 1px solid #ddd; }");
        sb.AppendLine("        tr:hover { background-color: #f8f9fa; }");
        sb.AppendLine("        .arrow { color: #e74c3c; font-weight: bold; }");
        sb.AppendLine("        tr.highlight { background-color: #e0e0e0; }");
        sb.AppendLine("        tr.highlight:hover { background-color: #d0d0d0; }");
        sb.AppendLine("        .summary { margin-top: 30px; padding: 20px; background-color: white; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
        sb.AppendLine("        .summary h2 { margin-top: 0; color: #2c3e50; }");
        sb.AppendLine("        .summary-item { padding: 8px 0; border-bottom: 1px solid #eee; }");
        sb.AppendLine("        .summary-item:last-child { border-bottom: none; }");
        sb.AppendLine("        .summary-label { font-weight: 600; display: inline-block; min-width: 300px; }");
        sb.AppendLine("        .summary-value { color: #e74c3c; font-weight: bold; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine($"    <h1>{System.Net.WebUtility.HtmlEncode(title)}</h1>");
        sb.AppendLine("    <table>");

        // Add header row
        sb.AppendLine("        <thead>");
        sb.AppendLine("            <tr>");
        foreach (string header in _headers)
        {
            sb.AppendLine($"                <th>{System.Net.WebUtility.HtmlEncode(header)}</th>");
        }
        sb.AppendLine("            </tr>");
        sb.AppendLine("        </thead>");

        // Add data rows
        sb.AppendLine("        <tbody>");
        foreach ((string[] row, string? cssClass) in _rows)
        {
            string trClass = !string.IsNullOrEmpty(cssClass) ? $" class=\"{cssClass}\"" : "";
            sb.AppendLine($"            <tr{trClass}>");
            foreach (string cell in row)
            {
                string encodedCell = System.Net.WebUtility.HtmlEncode(cell);
                // Highlight arrows for changed values
                if (encodedCell.Contains("→"))
                {
                    encodedCell = encodedCell.Replace("→", "<span class=\"arrow\">→</span>");
                }
                sb.AppendLine($"                <td>{encodedCell}</td>");
            }
            sb.AppendLine("            </tr>");
        }
        sb.AppendLine("        </tbody>");

        sb.AppendLine("    </table>");

        // Add summary statistics if provided
        if (summaryStats != null && summaryStats.Count > 0)
        {
            sb.AppendLine("    <div class=\"summary\">");
            sb.AppendLine("        <h2>Summary Statistics</h2>");
            foreach (KeyValuePair<string, string> stat in summaryStats)
            {
                sb.AppendLine("        <div class=\"summary-item\">");
                sb.AppendLine($"            <span class=\"summary-label\">{System.Net.WebUtility.HtmlEncode(stat.Key)}:</span>");
                sb.AppendLine($"            <span class=\"summary-value\">{System.Net.WebUtility.HtmlEncode(stat.Value)}</span>");
                sb.AppendLine("        </div>");
            }
            sb.AppendLine("    </div>");
        }

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    public void SaveAsHtml(string filePath, string title = "Total Service Discrepancies", Dictionary<string, string>? summaryStats = null)
    {
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, ToHtml(title, summaryStats));
    }
}
