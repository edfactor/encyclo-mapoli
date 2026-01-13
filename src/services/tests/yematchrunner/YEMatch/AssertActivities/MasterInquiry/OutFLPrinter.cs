namespace YEMatch.AssertActivities.MasterInquiry;

/* A  Markdown printer for OutFL rows.  Highlights conflicting rows. */
public static class OutFLPrinter
{
    private static bool _printedHeader;

    private static readonly string[] _headers =
    {
        "Version", "SSN", "Hours", "Years", "Enrl", "Begin Bal", "Begin Vest", "Current Bal", "Vesting Pct", "Vesting Amount", "Cont", "ETVA Amount", "Error Message"
    };

    private static readonly string _format =
        // Version | SSN    | Hours | Years | Enrl | Begin Bal | Begin Vest | Current Bal | Vesting Pct | Vesting Amount | Cont | ETVA Amount | Error Message |
        "| {0,-7} | {1,-12} | {2,10} | {3,6} | {4,4} | {5,13} | {6,14} | {7,15} | {8,15} | {9,15} | {10,5} | {11,15} | {12,-13} |";

    public static void PrintComparisonTable(Dictionary<string, int> diffCount, OutFL ready, OutFL smart)
    {
        if (!_printedHeader)
        {
            PrintHeader();
            _printedHeader = true;
        }

        ConsoleStock("READY", ready);

        string hours = ""; // OutFLComparer.MarkupIfDifferent(diffCount, "hours", ready.OUT_HRS, smart.OUT_HRS)
        string years = OutFLComparer.MarkupIfDifferent(diffCount, "years", ready.OUT_YEARS, smart.OUT_YEARS);
        string enrolled = OutFLComparer.MarkupIfDifferent(diffCount, "enrolled", ready.OUT_ENROLLED, smart.OUT_ENROLLED);
        string beginBal = OutFLComparer.MarkupIfDifferent(diffCount, "beginBal", ready.OUT_BEGIN_BAL, smart.OUT_BEGIN_BAL);
        string beginVest = OutFLComparer.MarkupIfDifferent(diffCount, "beginVest", ready.OUT_BEGIN_VEST, smart.OUT_BEGIN_VEST);
        string currentBal = OutFLComparer.MarkupIfDifferent(diffCount, "currentBal", ready.OUT_CURRENT_BAL, smart.OUT_CURRENT_BAL);
        string vestingPct = OutFLComparer.MarkupIfDifferent(diffCount, "vestingPct", ready.OUT_VESTING_PCT, smart.OUT_VESTING_PCT);
        string vestingAmt = OutFLComparer.MarkupIfDifferent(diffCount, "vestingAmt", ready.OUT_VESTING_AMT, smart.OUT_VESTING_AMT);
        string contrLast = OutFLComparer.MarkupIfDifferent(diffCount, "contLastYear", ready.OUT_CONT_LAST_YEAR, smart.OUT_CONT_LAST_YEAR);
        string etva = OutFLComparer.MarkupIfDifferent(diffCount, "etva", ready.OUT_ETVA, smart.OUT_ETVA);


        string[] values = new string[13];
        values[0] = "SMART";
        values[1] = smart.OUT_SSN;
        values[2] = hours;
        values[3] = years;
        values[4] = enrolled;
        values[5] = beginBal;
        values[6] = beginVest;
        values[7] = currentBal;
        values[8] = vestingPct;
        values[9] = vestingAmt;
        values[10] = contrLast;
        values[11] = etva;
        values[12] = smart.OUT_ERR_MESG;

        Console.WriteLine(_format, values);
    }


    public static void PrintHeader()
    {
        string[] dividers = _headers
            .Select(h => new string('-', Math.Max(h.Length, 3))) // make sure each column has at least 3 dashes
            .ToArray();

        Console.WriteLine(_format, _headers);
        Console.WriteLine(_format, dividers);
    }

    public static void ConsoleStock(string prefix, OutFL record)
    {
        string contLastYear = record.OUT_CONT_LAST_YEAR ? "YES" : "NO";

        Console.WriteLine(_format, prefix, record.OUT_SSN, record.OUT_HRS, record.OUT_YEARS, record.OUT_ENROLLED, record.OUT_BEGIN_BAL, record.OUT_BEGIN_VEST,
            record.OUT_CURRENT_BAL, record.OUT_VESTING_PCT, record.OUT_VESTING_AMT, contLastYear, record.OUT_ETVA, record.OUT_ERR_MESG);
    }

    public static void ConsoleMissing(string prefix)
    {
        Console.WriteLine(_format, prefix, "", "", "", "", "", "", "", "", "", "", "", "");
    }
}
