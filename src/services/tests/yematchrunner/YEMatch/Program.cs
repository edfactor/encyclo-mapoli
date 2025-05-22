using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

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

    private static async Task Main(string[] args)
    {
        Stopwatch wholeRunStopWatch = Stopwatch.StartNew();

        ActivityFactory.Initialize();

        // -------------------------------------------------
        // Activities are run in a myriad of ways depending on the demands of the moment.    
        // The current way of specifying what gets run is "lack luster" 
        // Some variations are;
        //  - Only SMART
        //  - Only Ready
        //  - only some of SMART and READY (like first 5 jobs)
        //  - run some of ready, then migrate ready 2 smart, then stop.
        // 
        // These variations should be codified in a clear structure, but they are ad-hoc at the moment

        List<IActivity> activitiesToRun = Specify([
            "P0", // Init READY / Init Smart
            
//            "Reduce-To-2-Employees",
            
            // Do a smart freeze on 01/04/2025
            "S12",
            
  //          "Fire-employee",
            
            // The obfuscation data is already at the FROZEN point (SHIFT already applied), so we start here
//            "P15",
//            "P16",
//            "P17",
            "P18", // PAY426 / YearEndService
           
            "Test-PayProfit-Selected-Columns", // Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate 

//            "P19",
//            "P20",
            "R21", // PAY444 - update intermediate values
            "R22", // PAY447 - creates a data file
            
            "P23", // Does Contributions

            "Test-ProfitDetail-Selected-Columns", // TEST: code,cont,earn,fort,cmt,zercont
            "Test-Etva-Now", // Verify ETVA for 2025
            "Test-Etva-Prior" // Verify correct ETVA for 2024

        ]);
        // activitiesToRun = Specify(bothReadyAndSmartActivities, ["R23"]);


        // ------------------- End of selection of steps zone

        // ensure we stop if a stop is requested on either side
        bool stopOnSmartSide = activitiesToRun.Any(a => a is SmartActivity);

        if (activitiesToRun.Any(a => a is SmartActivity && a != SmartActivityFactory.ReadyToSmartInit))
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

        // This could go into the log directory, but I like seeing in the ide... so for now I'm  leave it at the project root
        string filePath = $"../../../outcomes-{DateTime.Now:yyyyMMdd-HHmmss}.json";
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(outcomes));
        Console.WriteLine("Saved outcomes to file.");

        // Summary.print("latest");

        TimeSpan wholeRunElapsed = wholeRunStopWatch.Elapsed;
        Console.WriteLine($"\n---- Completed in Took:  {wholeRunElapsed.Hours}h {wholeRunElapsed.Minutes}m {wholeRunElapsed.Seconds}s");
    }

    private static List<IActivity> Specify(List<string> activityNames)
    {
        return activityNames.Select(name => ActivityFactory.AllActivtiesByName()[name]).ToList();
    }
}
