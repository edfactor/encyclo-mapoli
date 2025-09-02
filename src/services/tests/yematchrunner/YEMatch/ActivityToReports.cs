namespace YEMatch.YEMatch;

public static class ActivityToReports
{

    private static readonly Dictionary<string, string> _processIdByActivity = new()
    {
        ["R3"] = "7777",
        ["R2"] = "8888",
        ["R0"] = "12180",
        ["R15"] = "12228",
        ["R17"] = "12278",
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
        "PREVPROF-8888",
        "PREVPROF-8888.csv",
        "PROF-HOURS-DOLLARS-12228.CSV",
        "QPAY066-7777",
        "PAY426-12278",
        "PAY426-TOT-12278",
        "PAY426N-3-12278",
        "PAY426N-8-12278",
        "PAY426N-7-12278",
        "PAY426N-5-12278",
        "PAY426N-10-12278",
        "PAY426N-4-12278",
        "PAY426N-6-12278",
        "PAY426N-2-12278",
        "PAY426N-1-12278",
        "PAY426N-9-12278",
        "PAY426-24515",
        "PAY426N-7-24515",
        "PAY426N-6-24515",
        "PAY426N-5-24515",
        "PAY426N-1-24515",
        "PAY426N-3-24515",
        "PAY426N-4-24515",
        "PAY426N-2-24515",
        "PAY426N-9-24515",
        "PAY426N-10-24515",
        "PAY426N-8-24515",
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
