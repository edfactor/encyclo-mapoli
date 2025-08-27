using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;
using Renci.SshNet;
using YEMatch.YEMatch.ReadyActivities;

namespace YEMatch.YEMatch;

// Tries to parse the outcome.json and get all the remote files.    Used for creating the Golden Master
[SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions")]
public static class GetGold
{
    private static readonly Dictionary<string, string> _referenceJobByLogPid = new()
    {
        ["7777"] = "R3",
        ["8888"] = "R2",
        ["12180"] = "R0",
        ["12228"] = "R15",
        ["12278"] = "R17",
        ["24515"] = "R18",
        ["3719"] = "R19",
        ["7670"] = "R20",
        ["12105"] = "R21",
        ["20718"] = "R22",
        ["20799"] = "R23",
        ["21279"] = "R24",
        ["8855"] = "R24B",
        ["3110"] = "R25",
        ["8074"] = "R26",
        ["12201"] = "R27",
        ["19155"] = "R28"
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

    /* delete all the golden files.   Ensures we dont mix a prior run with a new run. */

    public static void Purge()
    {
        string goldenDir = $"{ReadyActivity.OptionalLocalResourceBase}golden";
        foreach (string file in Directory.GetFiles(goldenDir))
        {
            if (file.EndsWith("README.md"))
            {
                continue;
            }

            File.Delete(file);
        }
    }

    public static void Fetch(string dataDirectory, SftpClient sftpClient)
    {
        string goldenDir = $"{ReadyActivity.OptionalLocalResourceBase}golden";
        string outcomeFile = $"{dataDirectory}/outcome.json";
        if (File.Exists(outcomeFile))
        {
            File.Copy(outcomeFile, $"{goldenDir}/outcome.json", true);
        }

        string json = File.ReadAllText($"{goldenDir}/outcome.json");
        List<Outcome> outcomes = JsonSerializer.Deserialize<List<Outcome>>(json)!;

        Dictionary<string, string> activityByPid = new();
        foreach (Outcome outcome in outcomes)
        {
            // Find log file in output
            Match match = Regex.Match(outcome.StandardOut, @"LogFile:\s*(\S+)_(\S+)\.log");
            if (match.Success)
            {
                string logFilePath = match.Groups[2].Value;
                var activityName = outcome.ActivityLetterNumber.IndexOf("/") == -1
                    ? outcome.ActivityLetterNumber
                    : outcome.ActivityLetterNumber.Substring(0, outcome.ActivityLetterNumber.IndexOf("/"));
                activityByPid.Add(activityName, logFilePath);
                Console.WriteLine($"Mapped {activityName} to logfile {logFilePath}");
            }
        }

        int cnt = 1;
        foreach (string logFile in _referenceLogfiles)
        {
            (string rName, string sName, string activityName) = GetReferenceActivity(activityByPid, logFile);
            if (rName.Length == 0)
            {
                Console.WriteLine($"ActivityName {activityName} MISSING data from {logFile}");
                continue;
            }

            string shortName = $"{cnt:D2}-{activityName}-{sName}";
            Console.WriteLine($"{activityName} {cnt++} downloading {rName} ==> {shortName}");

            string readyFile = "/dsmdev/data/PAYROLL/SYS/PVTSYSOUT/" + rName;
            string smartFile = $"{goldenDir}/{shortName}";
            File.Delete(smartFile);
            using (FileStream fileStream = File.OpenWrite(smartFile))
            {
                sftpClient.DownloadFile(readyFile, fileStream);
            }

            cnt++;
        }

        Console.WriteLine("DONE");
    }

    private static (string readyName, string smartName, string activityName) GetReferenceActivity(Dictionary<string, string> realActivityByPid, string logFileName)
    {
        (string activityName, string oldPid) = GetReferenceActivityAndPid(logFileName);
        if (!realActivityByPid.ContainsKey(activityName))
        {
            return ("", "", "");
        }

        string newPid = realActivityByPid[activityName];
        return (logFileName.Replace(oldPid, newPid), logFileName.Replace("-" + oldPid, ""), activityName);
    }

    public static (string activityName, string oldPid) GetReferenceActivityAndPid(string logFileName)
    {
        string? matchingPid = null;
        string? matchingActivity = null;
        foreach (KeyValuePair<string, string> kvp in _referenceJobByLogPid)
        {
            //   key="PROF-HOURS-DOLLARS-12228.CSV" value=12228
            if (logFileName.Contains(kvp.Key))
            {
                if (matchingPid != null)
                {
                    throw new InvalidDataException("Two pids matched for a given LogFileName.  Impossible.");
                }

                matchingPid = kvp.Key;
                matchingActivity = kvp.Value;
            }
        }

        return (matchingActivity!, matchingPid!);
    }
}
