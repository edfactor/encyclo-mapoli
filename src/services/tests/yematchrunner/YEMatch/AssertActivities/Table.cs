using System.Text;

namespace YEMatch.AssertActivities;

public record Row(IReadOnlyDictionary<string, object?> Values);

public record Table(IReadOnlyList<Row> Rows)
{
    public override string ToString()
    {
        if (Rows.Count == 0)
        {
            return "(empty table)";
        }

        List<string> columnNames = Rows[0].Values.Keys.ToList();

        // compute max width per column
        Dictionary<string, int> widths = new();
        foreach (string col in columnNames)
        {
            int max = col.Length;
            foreach (Row row in Rows)
            {
                string text = row.Values[col]?.ToString() ?? "NULL";
                max = Math.Max(max, text.Length);
            }

            widths[col] = max;
        }

        StringBuilder sb = new();

        // header
        foreach (string col in columnNames)
        {
            sb.Append("| ").Append(col.PadRight(widths[col])).Append(' ');
        }

        sb.AppendLine("|");

        // separator
        foreach (string col in columnNames)
        {
            sb.Append("| ").Append(new string('-', widths[col])).Append(' ');
        }

        sb.AppendLine("|");

        // rows
        foreach (Row row in Rows)
        {
            foreach (string col in columnNames)
            {
                string text = row.Values[col]?.ToString() ?? "NULL";
                sb.Append("| ").Append(text.PadRight(widths[col])).Append(' ');
            }

            sb.AppendLine("|");
        }

        return sb.ToString();
    }
}
