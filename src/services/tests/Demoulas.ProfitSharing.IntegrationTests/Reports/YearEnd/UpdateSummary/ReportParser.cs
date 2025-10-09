namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.UpdateSummary;

// Parses a line of the READY Pay450 report
public static class ReportParser
{
    public static Pay450Record ParseLine(string line)
    {
        string badgeAndStore = line.Substring(0, 12).Trim();
        string name = line.Substring(12, 29).Trim();
        decimal beforeAmount = DecimalParse(line, 42);
        decimal beforeVested = DecimalParse(line, 58);
        int? beforeYears = IntegerParse(line, 72, 5);
        int? beforeEnroll = IntegerParse(line, 79, 5);
        decimal afterAmount = DecimalParse(line, 92);
        decimal afterVested = DecimalParse(line, 109);
        int? afterYears = IntegerParse(line, 126, 5);
        int? afterEnroll = IntegerParse(line, 131, -1);


        Pay450Record record = new()
        {
            BadgeAndStore = badgeAndStore,
            Name = name,
            BeforeAmount = beforeAmount,
            BeforeVested = beforeVested,
            BeforeYears = beforeYears,
            BeforeEnroll = beforeEnroll,
            AfterAmount = afterAmount,
            AfterVested = afterVested,
            AfterYears = afterYears,
            AfterEnroll = afterEnroll
        };
        return record;
    }


    private static int? IntegerParse(string line, int startPos, int width)
    {
        if (line.Length < startPos)
        {
            return null;
        }

        string numStr = (width == -1 ? line.Substring(startPos) : line.Substring(startPos, width)).Trim();
        if (numStr.Length == 0)
        {
            return null;
        }

        return int.Parse(numStr);
    }


    private static decimal DecimalParse(string data, int index)
    {
        string digits = data.Substring(index, 12).Trim();
        string sign = data.Length == index + 12 ? "" : data.Substring(index + 12, 1).Trim();
        if (digits + sign == "")
        {
            return 0;
        }

        return decimal.Parse(sign + digits);
    }
}
