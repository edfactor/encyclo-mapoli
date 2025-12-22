namespace YEMatch;

public static class ActivityToReports
{
    // Maps activity name to its generated report files (base names without PID)
    // Prefix with "=" for files that don't have a PID suffix (e.g., "=EXCELCONTN")
    private static readonly Dictionary<string, string[]> _reportsByActivity = new()
    {
        ["R01"] = [
            "ETVA-LESS-THAN-ZERO.csv",
            "DUPLICATE-PAYPROF-SSNS.csv",
            "DUPLICATE-DEM-SSNS.csv",
            "MISMATCHED-PAYPROF-DEM-SSNS.csv",
            "MISSING-DEMOGRAPHICS-RECS.csv",
            "MISSING-PAYPROFIT-RECS.csv",
            "DUP-NAMES-DOB.csv",
            "MISS-COMMA-IN-NAM.csv"
        ],
        ["R02"] = [
            "QPAY511",
            "PREVPROF",
            "PREVPROF.csv"
        ],
        ["R03"] = ["QPAY066", "QPAY066.csv"],
        ["R04"] = ["QPAY129"],
        ["R05"] = ["PROF-EXEC-HOURS-DOLLARS.CSV"],
        ["R08"] = [
            "PAY426-TOT",
            "PAY426",
            "PAY426N-1", "PAY426N-2", "PAY426N-3", "PAY426N-4", "PAY426N-5",
            "PAY426N-6", "PAY426N-7", "PAY426N-8", "PAY426N-9", "PAY426N-10"
        ],
        ["R11"] = ["PROF-HOURS-DOLLARS.CSV"],
        ["R13A"] = ["PROFIT-SHIFT-RPT"],
        ["R13B"] = ["PROFIT-SHIFT-RPT"],
        ["R15"] = ["PROF-HOURS-DOLLARS.CSV"],
        ["R17"] = [
            "PAY426-TOT",
            "PAY426",
            "PAY426N-1", "PAY426N-2", "PAY426N-3", "PAY426N-4", "PAY426N-5",
            "PAY426N-6", "PAY426N-7", "PAY426N-8", "PAY426N-9", "PAY426N-10"
        ],
        ["R18"] = [
            // Note: R18 (Final Run) does not generate PAY426-TOT unlike R8 and R17
            "PAY426",
            "PAY426N-1", "PAY426N-2", "PAY426N-3", "PAY426N-4", "PAY426N-5",
            "PAY426N-6", "PAY426N-7", "PAY426N-8", "PAY426N-9", "PAY426N-10"
        ],
        ["R19"] = ["PROFIT-ELIGIBLE.csv"],
        ["R20"] = ["PAY443"],
        ["R21"] = ["PAY444", "PAY444L"],
        ["R22"] = ["PAY447"],
        ["R24"] = ["PAY450", "PROF-CNTRL-SHEET"],
        ["R24B"] = ["PAY450", "PROF-CNTRL-SHEET"],
        ["R25"] = ["PROF130Y", "PROF130", "PROF130B", "PROF130V"],
        ["R26"] = ["QPAY501"],
        ["R27"] = ["QPAY066-UNDR21", "QPAY066TA-UNDR21", "QPAY066TA", "NEWPSLABELS", "=EXCELCONTN"],
        ["R28"] = ["SSNORDER", "QPAY066TA", "PAYCERT"]
    };

    public static List<(string Filename, string BaseName)> GetReportFilenamesForActivity(string activityName, string processId)
    {
        if (!_reportsByActivity.TryGetValue(activityName, out var patterns))
        {
            return [];
        }

        return patterns
            .Select(p =>
            {
                // Files prefixed with "=" don't get a PID suffix
                if (p.StartsWith('='))
                {
                    string baseName = p[1..];
                    return (Filename: baseName, BaseName: baseName);
                }

                return (Filename: InsertPid(p, processId), BaseName: p);
            })
            .ToList();
    }

    // Insert PID before extension if present, otherwise append
    private static string InsertPid(string pattern, string pid)
    {
        var dotIndex = pattern.LastIndexOf('.');
        return dotIndex >= 0
            ? $"{pattern[..dotIndex]}-{pid}{pattern[dotIndex..]}"
            : $"{pattern}-{pid}";
    }
}
