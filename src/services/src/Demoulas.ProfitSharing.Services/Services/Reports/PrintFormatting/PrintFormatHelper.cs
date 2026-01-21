using System.Text;

namespace Demoulas.ProfitSharing.Services.PrintFormatting;

public static class PrintFormatHelper
{
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
