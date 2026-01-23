using System.Text;

namespace Demoulas.ProfitSharing.Services.Services.Reports.PrintFormatting;

public static class PrintFormatHelper
{
    /// <summary>
    /// Appends a form feed followed by a CRLF to force a page break in print output.
    /// </summary>
    /// <param name="builder">The target <see cref="StringBuilder"/>.</param>
    public static void AppendPageBreak(StringBuilder builder)
    {
        if (builder.Length > 0)
        {
            var lastChar = builder[^1];
            if (lastChar == '\r')
            {
                _ = builder.Append('\n');
            }
            else if (lastChar != '\n')
            {
                _ = builder.Append("\r\n");
            }
        }

        _ = builder.Append("\f\r\n");
    }

    public static void AppendXeroxHeader(StringBuilder builder, string header, bool isXerox)
    {
        if (!isXerox)
        {
            return;
        }

        _ = builder.Append(header);
    }

    public static void AppendXeroxLine(StringBuilder builder, string line, bool isXerox)
    {
        if (!isXerox)
        {
            return;
        }

        _ = builder.AppendLine(line);
    }
}
