using System.Text;

namespace Demoulas.ProfitSharing.IntegrationTests.TotalSvc;

// Simple code for emitting a markdown table

public class MarkdownTable
{
    private readonly List<string> _headers;
    private readonly List<string[]> _rows;

    public MarkdownTable(string[] headers)
    {
        _headers = headers.ToList();
        _rows = new List<string[]>();
    }

    public void AddRow(params string[] columns)
    {
        if (columns.Length != _headers.Count)
        {
            throw new ArgumentException($"Expected {_headers.Count} columns, but got {columns.Length}.");
        }

        _rows.Add(columns);
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        // Add the header row
        sb.AppendLine("| " + string.Join(" | ", _headers) + " |");

        // Add the separator row
        sb.AppendLine("|" + string.Join("|", _headers.Select(_ => "---")) + "|");

        // Add data rows
        foreach (string[] row in _rows)
        {
            sb.AppendLine("| " + string.Join(" | ", row) + " |");
        }

        return sb.ToString();
    }

    public List<string[]> rows()
    {
        return _rows;
    }
}
