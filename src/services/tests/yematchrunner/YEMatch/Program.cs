using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace YEMatch;

#pragma warning disable S1144

[SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
[SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed")]
internal static class Program
{
    // Steps which modify the database (or input to next job which does) on READY 
    // A6 - Clear exec hours and dollars (only on ready)
    // A7 - Enter executive hours and dollars
    // A12 - Year end create DEMO_PROFSHARE / Frozen
    // A13A - PAY PROFIT SHIFT (only on ready)
    // A13B - PAY PROFIT SHIFT (only on ready)

    // A18 - Profit Share Report  <----------------------- Pay426 updates the Earned Points, ZeroContributionReason, New Employee, and CertificateDate
    // A20 - PAY443 - Profit Share Forfeit   (Report only)
    // A21 - Profit Share Update (updates PAYPROFIT shared values)
    // A22 - Profit Share Edit (writes file used by A23)
    // A23 - Profit Master Update
    // A24 - Updates enrollement_id 

#pragma warning disable AsyncFixer01

    private static string _dataDirectory = "";

    private static async Task Main(string[] args)
    {
        _dataDirectory = Config.CreateDataDirectory();
        ActivityFactory.Initialize(_dataDirectory);

        await Run(Specify(
            "R0",
            "DropBadBenes",
            "R2S",
            "S12", // Freeze on Smart
            "P18",  // Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate 
            "TestPayProfitSelectedColumns", // Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate 
            "R20" // Pay444 report
        ));
    }

    /*
     * Should probably become [Fact] test cases.
     */
    public static async Task Others()
    {
        await Run(Specify(
            "R0", // import obfuscated
            // "TrimTo14Employees", // Reduces execution time to 1 minute
            "R15",
            "R16",
            "R17",
            "R18",
            "R19",
            "R20",
            "R21"
        ));

        // Tests that Frozen is handled correctly by messing up the badges.
        await Run(Specify(
            "R0", // import obfuscated
            "TrimTo14Employees", // Reduces execution time to 1 minute
            "R2S",
            "S12", // Freeze on Smart
            "OverwriteBadges", // Obliterate the Live Badges
            "P18", // PAY426 / YearEndService
            "TestPayProfitSelectedColumns", // Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate 
            "R21", // PAY444 - update intermediate values
            "R22", // PAY447 - creates a data file
            "P23", // Does Contributions <-- smart does not yet use frozen - so it will fail
            "TestProfitDetailSelectedColumns", // TEST: code,cont,earn,fort,cmt,zercont
            "TestEtvaNow", // Verify ETVA for 2025
            "TestEtvaPrior" // Verify correct ETVA for 2024
        ));

        // Baseline correct
        await Run(Specify(
            "P0",
            "S12", // Freeze on Smart
            "P18",
            "TestPayProfitSelectedColumns", // Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate 
            "R21", // PAY444 - update intermediate values
            "R22", // PAY447 - creates a data file
            "P23", // Does Contributions
            "TestProfitDetailSelectedColumns", // TEST: code,cont,earn,fort,cmt,zercont
            "TestEtvaNow", // Verify ETVA for 2025
            "TestEtvaPrior" // Verify correct ETVA for 2024
        ));
    }
#pragma warning restore AsyncFixer01

    private static async Task Run(List<IActivity> activitiesToRun)
    {
        Stopwatch wholeRunStopWatch = Stopwatch.StartNew();
        if (activitiesToRun.Any(a => a is SmartActivity))
        {
            // Quick authentication sanity check
            AppVersionInfo? r = await SmartActivityFactory.Client!.DemoulasCommonApiEndpointsAppVersionInfoEndpointAsync(null);
            // Might be nice to also include the database version. What database is used.  Wall clock time.
            Console.WriteLine(" Connected to SMART build:" + r.BuildNumber + " git-hash:" + r.ShortGitHash);
        }

        List<Outcome> outcomes = [];
        foreach (IActivity activity in activitiesToRun)
        {
            Console.WriteLine($"------------------- Starting execution: {activity.Name()}");
            Outcome outcome = await activity.Execute();
            outcomes.Add(outcome);
            string msg = outcome.Message.Replace("\n", "\n" + "   ").Trim();
            if (msg.Length != 0)
            {
                Console.WriteLine($"   {msg}");
            }

            Console.WriteLine($"   Took: {outcome.took}    Status: {outcome.Status}");

            if (outcome.Status == OutcomeStatus.Error)
            {
                Console.WriteLine("------------------- Stopping execution due to error/failure: " + outcome.Name);
                break;
            }
        }

        string filePath = $"{_dataDirectory}/outcome.json";
        var options = new JsonSerializerOptions { WriteIndented = true };
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(outcomes,options));

        TimeSpan wholeRunElapsed = wholeRunStopWatch.Elapsed;
        Console.WriteLine($"\n---- Completed YERunner.  Took:  {wholeRunElapsed.Hours}h {wholeRunElapsed.Minutes}m {wholeRunElapsed.Seconds}s");
    }

    private static List<IActivity> Specify(params List<string> activityNames)
    {
        return activityNames.Select(name => ActivityFactory.AllActivtiesByName()[name]).ToList();
    }
}
