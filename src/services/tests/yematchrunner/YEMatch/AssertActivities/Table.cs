using System.Text;

namespace YEMatch.YEMatch.AssertActivities;

public record Row(IReadOnlyDictionary<string, object?> Values);

public record Table(IReadOnlyList<Row> Rows)
{
    public override string ToString()
    {
        if (Rows.Count == 0)
        {
            return "(empty table)";
        }

        var columnNames = Rows[0].Values.Keys.ToList();

        // compute max width per column
        var widths = new Dictionary<string, int>();
        foreach (var col in columnNames)
        {
            int max = col.Length;
            foreach (var row in Rows)
            {
                var text = row.Values[col]?.ToString() ?? "NULL";
                max = Math.Max(max, text.Length);
            }
            widths[col] = max;
        }

        var sb = new StringBuilder();

        // header
        foreach (var col in columnNames)
        {
            sb.Append("| ").Append(col.PadRight(widths[col])).Append(' ');
        }

        sb.AppendLine("|");

        // separator
        foreach (var col in columnNames)
        {
            sb.Append("| ").Append(new string('-', widths[col])).Append(' ');
        }

        sb.AppendLine("|");

        // rows
        foreach (var row in Rows)
        {
            foreach (var col in columnNames)
            {
                var text = row.Values[col]?.ToString() ?? "NULL";
                sb.Append("| ").Append(text.PadRight(widths[col])).Append(' ');
            }
            sb.AppendLine("|");
        }

        return sb.ToString();
    }
}
