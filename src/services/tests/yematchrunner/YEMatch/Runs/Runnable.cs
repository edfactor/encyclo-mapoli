using System.Diagnostics;
using System.Text.Json;
using YEMatch.YEMatch.Activities;
using YEMatch.YEMatch.SmartActivities;

namespace YEMatch.YEMatch.Runs;

/*
 * Base class for executing "Runs".   Runs are a series of Tasks (activity or tests or setup steps) which are used to move READY and/or SMART though different scenarioss.
 */
public abstract class Runnable
{
    public required string DataDirectory { get; set; }

    public bool CompletedWithoutError { get; set; } = true;

    public abstract Task Exec();

    protected async Task Run(List<IActivity> activitiesToRun)
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
            Stopwatch wholeActivityStopWatch = Stopwatch.StartNew();
            Console.WriteLine($"------------------- Starting execution: {activity.Name()}");
            Outcome outcome = await activity.Execute();
            outcomes.Add(outcome);
            string msg = outcome.Message.Replace("\n", "\n" + "   ").Trim();
            if (msg.Length != 0)
            {
                Console.WriteLine($"   {msg}");
            }

            Console.WriteLine($"   Took: {outcome.took}    Status: {outcome.Status}      WholeTime: {wholeRunStopWatch.Elapsed}");

            if (outcome.Status == OutcomeStatus.Error)
            {
             
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("------------------- Stopping execution due to error/failure: " + outcome.Name);
                Console.ResetColor();
                CompletedWithoutError = false;
                break;
            }
        }

        string filePath = $"{DataDirectory}/outcome.json";
        JsonSerializerOptions options = new() { WriteIndented = true };
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(outcomes, options));

        TimeSpan wholeRunElapsed = wholeRunStopWatch.Elapsed;
        Console.WriteLine($"\n---- Completed YERunner.  Took:  {wholeRunElapsed.Hours}h {wholeRunElapsed.Minutes}m {wholeRunElapsed.Seconds}s");
            
        Process.Start("afplay", "/System/Library/Sounds/Submarine.aiff");
    }

    protected static List<IActivity> Specify(params List<string> activityNames)
    {
        return activityNames.ConvertAll(name => ActivityFactory.AllActivtiesByName()[name]);
    }
}
