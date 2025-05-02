using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using YEMatch.YEMatch;

namespace YEMatch;

[SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
[SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed")]
internal static class Program
{
    private static readonly HashSet<string> activitiesWithUpdates = ["A6", "A7", "A12", "A13A", "A13B", "A18", "A20", "A23", "A24"];
    
    // Steps which modify the database
    // A6 - Clear exec hours and dollars (only on ready)
    // A7 - Enter executive hours and dollars
    // A12 - Year end create DEMO_PROFSHARE / Frozen
    // A13A - PAY PROFIT SHIFT (only on ready)
    // A13B - PAY PROFIT SHIFT (only on ready)
    // A18 - Profit Share Report  <----------------------- Pay426 updates the Earned Points, ZeroContributionReason 
    // A20 - PAY443 - Profit Share Forfeit   
    // A23 - ......
    // A24 - Updates enrollement_id 

    private static async Task Main(string[] args)
    {
        if (args.Length == 1)
        {
            Summary.print(args[0]);
            return;
        }

        // used to get desired BaseDataDirectory for writing log files
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        string baseDir = config["BaseDataDirectory"] ?? Path.Combine("/tmp", "ye");
        Directory.CreateDirectory(baseDir);

        string dataDirectory = Path.Combine(baseDir, $"{DateTime.Now:dd-MMM-HH-mm}");
        Directory.CreateDirectory(dataDirectory);
        Console.WriteLine($"Directory created: file:///{dataDirectory}");

        var wholeRunStopWatch = new Stopwatch();
        wholeRunStopWatch.Start();

        var header = "|------------  READY -------------------------|   |------------------  SMART -------------------------- |";
        Console.WriteLine(header);
        var prefix = header.IndexOf(" |");

        var smartActivities = SmartActivityFactory.CreateActivities(dataDirectory);
        var readyActivities = ReadyActivityFactory.CreateActivities(dataDirectory);

        if (readyActivities.Count != smartActivities.Count)
        {
            throw new InvalidOperationException("READY and SMART activities are different length");
        }

        for (var i = 0; i < readyActivities.Count; i++)
        {
            if (smartActivities[i].ActivityLetterNumber != readyActivities[i].ActivityLetterNumber)
                // We always expect Ready short name to be the same as Smart short name (ie.  A7 and A7 should match.)
            {
                throw new InvalidOperationException(
                    $"READY and SMART activities are different at index {i}  s={smartActivities[i].ActivityLetterNumber} r={readyActivities[i].ActivityLetterNumber}");
            }
        }

        // -------------------------------------------------
        // Activities are run in a varietity of ways depending on the demands of the moment.    
        // The current way of specifyng what gets run is "lack luster" 
        // Some variations are;
        //  - Only SMART
        //  - Only Ready
        //  - only some of SMART and READY (like first 5 jobs)
        //  - run some of ready, then migrate ready 2 smart, then stop.
        // 
        // These variations should be codified in a clear structure, but they are ad-hoc at the moment

        // This block lets you skip steps when doing specific testing
        var skipBoth = 0;

        var startOnStep = "A18"; // step to start with, ie. "A10"
        var stopOnStep = "A23"; // step to start with, ie. "A10"

        startOnStep = "A0"; // step to start with, ie. "A10"
        stopOnStep = "A0"; // step to start with, ie. "A10"


        // ------------------- Start of selection of steps zone

        // TBD, should stop by name not index - so we can stop on either side (if running only 1 side)
        for (var i = 0; i < readyActivities.Count; i++)
        {
            if (smartActivities[i].ActivityLetterNumber == startOnStep)
            {
                skipBoth = i;
            }
        }

        readyActivities = readyActivities.Skip(skipBoth).Take(999).ToList();
        smartActivities = smartActivities.Skip(skipBoth).Take(999).ToList();

        // Only do two steps, A0 and A21
        // readyActivities = readyActivities.Where(a => a.ActivityLetterNumber is "A0" or "A3").ToList();
        // smartActivities = smartActivities.Where(a => a.ActivityLetterNumber is "A0" or "A3").ToList();

        var bothReadyAndSmartActivities = readyActivities.Zip(smartActivities, (a, b) => new[] { a, b })
            .SelectMany(x => x)
            .ToList();

        // Adjust this select ready/smart jobs
        var activitiesToRun = readyActivities;

        // Specfiy selects out a specific set of activities to run.   The "RX" ones are ready activities, and the "SX" ones are smart activities.
        // Test ProfitMaster/PROFTLB on the smart side 
        // THE "R2S" is a special case, it is an activity which clones the READY database into SMART
        activitiesToRun = Specify(bothReadyAndSmartActivities, ["R0", "R18", "R20", "R21", "R22", "R2S" /* "R23", "S23"*/]);
        // activitiesToRun = Specify(bothReadyAndSmartActivities, ["R23"]);


        // ------------------- End of selection of steps zone

        // ensure we stop if a stop is requested on either side
        bool stopOnSmartSide = activitiesToRun.Any(a => a is SmartActivity);
        
        if(activitiesToRun.Any(a=> a is SmartActivity && a != SmartActivityFactory.readyToSmartInit )){
            // Quick authentication sanity check
            var r = await SmartActivityFactory.client!.DemoulasCommonApiEndpointsAppVersionInfoEndpointAsync(null);
            // Might be nice to also include the database version. What database is used.  Wall clock time.
            Console.WriteLine($"{SmartActivity.smartPrefix}Connected to SMART build:" + r.BuildNumber + " git-hash:" + r.ShortGitHash);
        }

        List<Outcome> outcomes = [];
        foreach (var activity in activitiesToRun)
        {
            var outcome = await activity.execute();
            outcomes.Add(outcome);
            var msg = outcome.Message.Replace("\n", "\n" + activity.prefix + "   ").Trim();
            if (msg.Length != 0)
            {
                Console.WriteLine($"{ModChar(activity)}{activity.prefix}   {msg}");
            }

            Console.WriteLine($"{ModChar(activity)}{activity.prefix}   Took: {outcome.took}    Status: {outcome.Status}");

            if (outcome.Status == OutcomeStatus.Error)
            {
                Console.WriteLine("Stopping execution due to error.");
                break;
            }

            if (activity.ActivityLetterNumber == stopOnStep && stopOnSmartSide == outcome.isSmart)
            {
                Console.WriteLine($"Stopping as Requested. stopOnStep: {stopOnStep}, stop on smart side: {stopOnSmartSide}");
                break;
            }
        }

        var wholeRunElapsed = wholeRunStopWatch.Elapsed;

        // This could go into the log directory, but I like seeing in the ide... so for now I'm gonna leave it at the project root
        string filePath = $"../../../outcomes-{DateTime.Now:yyyyMMdd-HHmmss}.json";
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(outcomes));
        Console.WriteLine("Saved outcomes to file.");

        Summary.print("latest");

        Console.WriteLine($"\n---- Completed in Took:  {wholeRunElapsed.Hours}h {wholeRunElapsed.Minutes}m {wholeRunElapsed.Seconds}s");
    }

    private static List<Activity> Specify(List<Activity> bothReadyAndSmartActivities, List<string> activityNames)
    {
        var activitiesToRun = new List<Activity>();

        foreach (var descriptor in activityNames)
        {
            if (descriptor.Length < 2 || (descriptor[0] != 'R' && descriptor[0] != 'S'))
            {
                throw new ArgumentException($"Invalid activity descriptor: {descriptor}");
            }

            bool isReady = descriptor[0] == 'R';
            string key = descriptor.Substring(1);

            var match = bothReadyAndSmartActivities.FirstOrDefault(a =>
                a.ActivityLetterNumber.Substring(1) == key &&
                ((isReady && a is ReadyActivity) || (!isReady && a is SmartActivity)));

            if (match == null)
            {
                if (isReady && key == "2S")
                {
                    match = SmartActivityFactory.readyToSmartInit!;
                }
                else
                {
                    throw new ArgumentException($"I don't know how to: {descriptor}");
                }
            }

            activitiesToRun.Add(match);
        }

        return activitiesToRun;
    }


    public static string ModChar(Activity activity)
    {
        return activitiesWithUpdates.Contains(activity.ActivityLetterNumber) ? "*" : " ";
    }

    public static string ModChar(Outcome outcome)
    {
        return activitiesWithUpdates.Contains(outcome.ActivityLetterNumber) ? "*" : " ";
    }
}
