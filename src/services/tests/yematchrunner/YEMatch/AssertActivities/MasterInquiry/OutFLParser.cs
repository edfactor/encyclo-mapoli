namespace YEMatch.AssertActivities.MasterInquiry;

/* Parses a line from the cobol output file "OUTFL".  It contains a dump of values which match Master Inqiery Scrrens. */
public static class OutFLParser
{
    public static List<OutFL> ParseStringIntoRecords(string awData)
    {
        List<OutFL> records = new();
        string[] lines = awData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            records.Add(ParseLine(line));
        }

        return records;
    }

    public static OutFL ParseLine(string data)
    {
        string ssn = data[..9];
        decimal hrs = decimal.Parse(data.Substring(11, 7));
        int years = int.Parse(data.Substring(20, 2));
        string enrolled = data.Substring(24, 2).Trim();
        decimal beginBal = DecimalParse(data, 29);
        decimal beginVest = DecimalParse(data, 44);
        decimal currentBal = DecimalParse(data, 59);
        decimal vestingPct = decimal.Parse(data.Substring(73, 4));
        decimal vestingAmt = DecimalParse(data, 79);
        bool contrLastYear = data.Substring(94, 2) == "YE";

        decimal etva = 0;
        if (data.Length > 104 && data.Substring(104, 12) != "NEFICIARY **")
        {
            etva = DecimalParse(data, 104);
        }

        string errMsg = data.Length < 117 ? "" : data[117..];

        return new OutFL
        {
            OUT_SSN = ssn,
            OUT_HRS = hrs,
            OUT_YEARS = years,
            OUT_ENROLLED = enrolled,
            OUT_BEGIN_BAL = beginBal,
            OUT_BEGIN_VEST = beginVest,
            OUT_CURRENT_BAL = currentBal,
            OUT_VESTING_PCT = vestingPct,
            OUT_VESTING_AMT = vestingAmt,
            OUT_CONT_LAST_YEAR = contrLastYear,
            OUT_ETVA = etva,
            OUT_ERR_MESG = errMsg
        };
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
