using System.Text;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.Termination;

/// <summary>
///     Creates ASCII-formatted tables with borders and aligned columns.
/// </summary>
/// <example>
///     Usage:
///     <code>
/// var table = new AsciiTable("Name", "Age", "Score");
/// table.Add("Alice", 30, 95.5);
/// table.Add("Bob", 25, 87.3);
/// Console.WriteLine(table);
///
/// // Output:
/// // o-------o-----o-------o
/// // | Name  | Age | Score |
/// // o-------o-----o-------o
/// // | Alice |  30 |  95.5 |
/// // | Bob   |  25 |  87.3 |
/// // o-------o-----o-------o
/// </code>
/// </example>
public class AsciiTable
{
    private readonly string[] _headers;
    private readonly List<object?[]> _rows;

    public AsciiTable(params string[] headers)
    {
        _headers = headers;
        _rows = new List<object?[]>();
    }

    public int RowCount => _rows.Count;

    public void Add(params object?[] values)
    {
        if (values.Length != _headers.Length)
        {
            throw new ArgumentException($"Expected {_headers.Length} values, got {values.Length}");
        }

        _rows.Add(values);
    }

    public override string ToString()
    {
        // Calculate column widths
        int[] widths = new int[_headers.Length];

        // Initialize with header widths
        for (int i = 0; i < _headers.Length; i++)
        {
            widths[i] = _headers[i].Length;
        }

        // Check all row values
        foreach (object?[] row in _rows)
        {
            for (int i = 0; i < row.Length; i++)
            {
                int valueWidth = row[i]?.ToString()?.Length ?? 0;
                widths[i] = Math.Max(widths[i], valueWidth);
            }
        }

        // Build the table
        StringBuilder result = new();

        // Top border
        result.AppendLine(CreateBorder(widths));

        // Headers
        result.AppendLine(CreateRow(_headers.Select(h => (object)h).ToArray(), widths, false));

        // Middle border
        result.AppendLine(CreateBorder(widths));

        // Data rows
        foreach (object?[] row in _rows)
        {
            result.AppendLine(CreateRow(row, widths, true));
        }

        // Bottom border
        result.AppendLine(CreateBorder(widths));

        return result.ToString();
    }

    private static string CreateBorder(int[] widths)
    {
        IEnumerable<string> parts = widths.Select(w => new string('-', w + 2));
        return "o" + string.Join("o", parts) + "o";
    }

    private static string CreateRow(object?[] values, int[] widths, bool rightAlignNumbers)
    {
        List<string> parts = new();

        for (int i = 0; i < values.Length; i++)
        {
            string valueStr = values[i]?.ToString() ?? "";
            bool isNumber = rightAlignNumbers && values[i] != null && IsNumeric(values[i]);

            string formatted = isNumber
                ? valueStr.PadLeft(widths[i])
                : valueStr.PadRight(widths[i]);

            parts.Add(" " + formatted + " ");
        }

        return "|" + string.Join("|", parts) + "|";
    }

    private static bool IsNumeric(object? value)
    {
        return value is sbyte || value is byte || value is short || value is ushort ||
               value is int || value is uint || value is long || value is ulong ||
               value is float || value is double || value is decimal;
    }
}
