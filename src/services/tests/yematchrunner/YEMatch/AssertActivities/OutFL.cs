using System.Diagnostics.CodeAnalysis;

namespace YEMatch;

public sealed record OutFL
{
    private static bool Once;
    public required string OUT_SSN { get; set; } = "";
    public required decimal OUT_HRS { get; set; }
    public required int OUT_YEARS { get; set; }
    public required string OUT_ENROLLED { get; set; } = "";
    public required decimal OUT_BEGIN_BAL { get; set; }
    public required decimal OUT_BEGIN_VEST { get; set; }
    public required decimal OUT_CURRENT_BAL { get; set; }
    public required decimal OUT_VESTING_PCT { get; set; }
    public required decimal OUT_VESTING_AMT { get; set; }
    public required string OUT_ERR_MESG { get; set; } = "";

    public static List<OutFL> ParseStringIntoRecords(string awData)
    {
        List<OutFL> records = new();
        string[] lines = awData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            records.Add(parseLine(line));
        }

        return records;
    }

    public static void PrintComparisonTable(OutFL ready, OutFL smart)
    {
        HeaderIfNeeded();

        ConsoleColor defaultColor = Console.ForegroundColor;
        ready.ConsoleStock("READY");
        if (!IsSame(ready, smart))
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        string hours = MarkupIfDifferent(ready.OUT_HRS, smart.OUT_HRS);
        string years = MarkupIfDifferent(ready.OUT_YEARS, smart.OUT_YEARS);
        string enrolled = MarkupIfDifferent(ready.OUT_ENROLLED, smart.OUT_ENROLLED);
        string beginBal = MarkupIfDifferent(ready.OUT_BEGIN_BAL, smart.OUT_BEGIN_BAL);
        string beginVest = MarkupIfDifferent(ready.OUT_BEGIN_VEST, smart.OUT_BEGIN_VEST);
        string currentBal = MarkupIfDifferent(ready.OUT_CURRENT_BAL, smart.OUT_CURRENT_BAL);
        string vestingPct = MarkupIfDifferent(ready.OUT_VESTING_PCT, smart.OUT_VESTING_PCT);
        string vestingAmt = MarkupIfDifferent(ready.OUT_VESTING_AMT, smart.OUT_VESTING_AMT);

        Console.WriteLine(
            $"| {"SMART",-7} | {smart.OUT_SSN,-12} | {hours,8} | {years,9} | {enrolled,-12} | {beginBal,13} | {beginVest,14} | {currentBal,15} | {vestingPct,15} | {vestingAmt,15} | {smart.OUT_ERR_MESG,-12} |");

        Console.ForegroundColor = defaultColor;
    }

    public static void HeaderIfNeeded()
    {
        if (!Once)
        {
            // Header
            Console.WriteLine("");
            Console.WriteLine(
                "| Version | SSN          | Hours    | Years     | Enrolled     | Begin Bal     | Begin Vest     | Current Bal     | Vesting Pct     | Vesting Amount  | Error Message |");
            Console.WriteLine(
                "|---------|--------------|----------|-----------|--------------|---------------|----------------|-----------------|-----------------|-----------------|--------------|");
            Once = true;
        }
    }

    public void ConsoleStock(string prefix)
    {
        OutFL record = this;
        Console.WriteLine(
            $"| {prefix,-7} | {record.OUT_SSN,-12} | {record.OUT_HRS,8:N2} | {record.OUT_YEARS,9} | {record.OUT_ENROLLED,-12} | {record.OUT_BEGIN_BAL,13:N2} | {record.OUT_BEGIN_VEST,14:N2} | {record.OUT_CURRENT_BAL,15:N2} | {record.OUT_VESTING_PCT,15:F2} | {record.OUT_VESTING_AMT,15:N2} | {record.OUT_ERR_MESG,-12} |");
    }

    public static void ConsoleMissing(string prefix)
    {
        ConsoleColor defaultColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"| {prefix,-7} | {"",-12} | {"",8} | {"",9} | {"",-12} | {"",13} | {"",14} | {"",15} | {"",15} | {"",15} | {"",-12} |");
        Console.ForegroundColor = defaultColor;
    }

    private static string MarkupIfDifferent(object readyValue, object smartValue)
    {
        if (readyValue is null || smartValue is null)
        {
            return (readyValue?.ToString() ?? "") == (smartValue?.ToString() ?? "")
                ? readyValue?.ToString() ?? ""
                : $"⚠️ Null/NotNull/Blank";
        }

        if (readyValue.GetType() != smartValue.GetType())
        {
            // Types differ, consider them different
            return $"⚠️ TYPE {FormatValue(smartValue)}";
        }

        bool areEqual = readyValue switch
        {
            decimal d => Math.Round(d, 2) == Math.Round((decimal)smartValue, 2),
            int i => i == (int)smartValue,
            string s => s == (string)smartValue,
            _ => readyValue.Equals(smartValue)
        };

        string formatted = FormatValue(smartValue);

        return areEqual ? formatted : $"⚠️{formatted}";
    }

    private static string FormatValue(object? value)
    {
        if (value is null)
        {
            return "";
        }

        return value switch
        {
            decimal d => d.ToString("N2"),
            int i => i.ToString(),
            string s => s,
            _ => value.ToString() ?? ""
        };
    }


    public static bool IsSame(OutFL ready, OutFL smart)
    {
        return Normalize(ready).Equals(Normalize(smart));
    }

    // SMART gives us amounts with three decimal places,   44.443  we trim those here
    public static OutFL Normalize(OutFL record)
    {
        return record with
        {
            OUT_SSN = "", // since smart tramples over the ssn
            OUT_ERR_MESG = "", // since smart only returns a number...
            OUT_HRS = Math.Round(record.OUT_HRS, 2),
            OUT_ENROLLED = record.OUT_ENROLLED.Trim(),
            OUT_BEGIN_BAL = 0m, // Math.Round(record.OUT_BEGIN_BAL, 2),
            OUT_BEGIN_VEST = 0m, // Math.Round(record.OUT_BEGIN_VEST, 2),
            OUT_CURRENT_BAL = Math.Round(record.OUT_CURRENT_BAL, 2),
            OUT_VESTING_PCT = Math.Round(record.OUT_VESTING_PCT, 2),
            OUT_VESTING_AMT = Math.Round(record.OUT_VESTING_AMT, 2)
        };
    }


    public static OutFL parseLine(string data)
    {
        // OUTFL file looks like this;
        // string data = "700000072  0000.00  03  *4  000000955.09   000000955.09   000000955.09   1.00  000000955.09"
        
        // Field 1: 700000041
        string field1 = data.Substring(0, 9); // Starts at column 1

        // Field 2: 0071.00
        string field2 = data.Substring(11, 7); // Starts at column 12

        // Field 3: 07
        string field3 = data.Substring(20, 2); // Starts at column 21

        // Field 4: *2
        string field4 = data.Substring(24, 2); // Starts at column 25

        // Field 5: 000000000.00
        string field5 = data.Substring(29, 12); // Starts at column 30

        // Field 6: 000000000.00
        string field6 = data.Substring(44, 12); // Starts at column 45

        // Field 7: 000000000.00
        string field7 = data.Substring(59, 12); // Starts at column 60

        // Field 8: 1.00
        string field8 = data.Substring(73, 4); // Starts at column 74

        // Field 9: 000000000.00
        string field9 = data.Substring(79, 12); // Starts at column 80

        // Field 10: PROFIT SHARING MEMBER IS DECEASED - NOW 100% VESTED
        string field10 = data.Length < 94 ? "" : data[94..];

        return new OutFL
        {
            OUT_SSN = field1,
            OUT_HRS = decimal.Parse(field2),
            OUT_YEARS = int.Parse(field3),
            OUT_ENROLLED = field4,
            OUT_BEGIN_BAL = decimal.Parse(field5),
            OUT_BEGIN_VEST = decimal.Parse(field6),
            OUT_CURRENT_BAL = decimal.Parse(field7),
            OUT_VESTING_PCT = decimal.Parse(field8),
            OUT_VESTING_AMT = decimal.Parse(field9),
            OUT_ERR_MESG = field10
        };
    }
}
