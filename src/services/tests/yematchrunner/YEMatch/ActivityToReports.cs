namespace YEMatch.YEMatch;

public static class ActivityToReports
{
    // This maps a Activty Name to a unix proccess id.   as orginally extracted from a YE Run.
    private static readonly Dictionary<string, string> _processIdByActivity = new()
    {
        ["R1"] = "13604",
        ["R2"] = "13694",
        ["R3"] = "7777",
        ["R4"] = "7217",
        ["R5"] = "18007", // EJR PROF-DOLLAR-EXEC-EXTRACT
        // ["R6"] Clear EXEC hours dollars
        // ["R7"] Ready Screen
        ["R8"] = "18182", // PROF-SHARE PAY426 ...
        // ["R9"] YE-Oracle-Payroll-Processing
        // ["R10"] Load-Oracle-PAYPROFIT(weekly job)
        ["R11"] = "564", // A11: Profit sharing YTD Wages Extract (PROF-DOLLAR-EXTRACT)
        // ["R12"] =  PROF-LOAD-YREND-DEMO-PROFSHARE

        ["R15"] = "12228",
        ["R17"] = "18182",
        ["R18"] = "24515",
        ["R19"] = "3719",
        ["R20"] = "7670",
        ["R21"] = "12105",
        ["R22"] = "20718",
        ["R23"] = "20799",
        ["R24"] = "21279",
        ["R24B"] = "8855",
        ["R25"] = "3110",
        ["R26"] = "8074",
        ["R27"] = "12201",
        ["R28"] = "19155"
    };

    private static readonly List<string> _referenceLogfiles =
    [
        // These are names of generated reports.  Then end with a unix process id.  This keeps the reports
        // unique in a directory of reports.     We use this map to figure out for a given activity, like R22 which has
        // a single specific unix id assigned at run time, what file or files to go grab on the remote server.
        
        "ETVA-LESS-THAN-ZERO-13604.csv",
        "DUPLICATE-PAYPROF-SSNS-13604.csv",
        "DUPLICATE-DEM-SSNS-13604.csv",
        "MISMATCHED-PAYPROF-DEM-SSNS-13604.csv",
        "MISSING-DEMOGRAPHICS-RECS-13604.csv",
        "MISSING-PAYPROFIT-RECS-13604.csv",
        "DUP-NAMES-DOB-13604.csv",
        "MISS-COMMA-IN-NAM-13604.csv",
        "QPAY511-13694",
        "PREVPROF-13694",
        "PREVPROF-13694.csv",
        "QPAY066-7777",
        "QPAY129-7217",
        "PROF-EXEC-HOURS-DOLLARS-18007.CSV",
        "PAY426-TOT-18182",
        "PAY426-18182",
        "PAY426N-6-18182",
        "PAY426N-2-18182",
        "PAY426N-1-18182",
        "PAY426N-10-18182",
        "PAY426N-8-18182",
        "PAY426N-7-18182",
        "PAY426N-3-18182",
        "PAY426N-5-18182",
        "PAY426N-4-18182",
        "PAY426N-9-18182",
        "PROF-HOURS-DOLLARS-564.CSV",
        "PROFIT-ELIGIBLE-3719.csv",
        "PAY443-7670",
        "PAY444-12105",
        "PAY444L-12105",
        "PAY447-20718",
        "PAY450-21279",
        "PROF-CNTRL-SHEET-21279",
        "PROF-CNTRL-SHEET-8855",
        "PROF130Y-3110",
        "PROF130-3110",
        "PROF130B-3110",
        "PROF130V-3110",
        "QPAY501-8074",
        "QPAY066-UNDR21-12201",
        "QPAY066TA-UNDR21-12201",
        "QPAY066TA-12201",
        "NEWPSLABELS-12201",
        "SSNORDER-19155",
        "QPAY066TA-19155",
        "PAYCERT-19155"
    ];

    public static List<(string, string)> GetReportFilenamesForActivity(string activityName, string processId)
    {
        if (!_processIdByActivity.ContainsKey(activityName))
        {
            return new();
        }

        string referenceProcessId = _processIdByActivity[activityName];
        return _referenceLogfiles.Where(r => r.Contains(referenceProcessId)).Select(r => (r.Replace(referenceProcessId, processId), r.Replace("-" + referenceProcessId, ""))).ToList();
    }
}
