using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;

namespace YEMatch.Runs;

/*
 * Base class for executing "Runs".   Runs are a series of Tasks (activity or tests or setup steps) which are used to move READY and/or SMART though different scenarioss.
 */
public abstract class Runnable
{
    protected readonly IActivityFactory ActivityFactory;
    protected readonly ILogger Logger;
    protected readonly IReadySshClientFactory ReadySshClientFactory;
    protected readonly ISmartApiClientFactory SmartApiClientFactory;

    /// <summary>
    ///     Constructor for DI
    /// </summary>
    protected Runnable(
        IActivityFactory activityFactory,
        IReadySshClientFactory readySshClientFactory,
        ISmartApiClientFactory smartApiClientFactory,
        ILogger logger)
    {
        ActivityFactory = activityFactory ?? throw new ArgumentNullException(nameof(activityFactory));
        ReadySshClientFactory = readySshClientFactory ?? throw new ArgumentNullException(nameof(readySshClientFactory));
        SmartApiClientFactory = smartApiClientFactory ?? throw new ArgumentNullException(nameof(smartApiClientFactory));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Data directory for logs and output. Set by Program.cs during transition.
    /// </summary>
    public required string DataDirectory { get; set; }

    public bool CompletedWithoutError { get; set; } = true;

    public abstract Task Exec();

    protected async Task Run(List<IActivity> activitiesToRun)
    {
        Stopwatch wholeRunStopWatch = Stopwatch.StartNew();
        Logger.LogInformation("Starting run with {ActivityCount} activities", activitiesToRun.Count);
        Console.WriteLine($"Starting run with {activitiesToRun.Count} activities. Press 'S' or 's' at any time to stop gracefully (no enter needed).");
        Console.WriteLine();

        if (activitiesToRun.Any(a => a is SmartActivity))
        {
            // Quick authentication sanity check
            ApiClient client = SmartApiClientFactory.CreateClient();
            IAppVersionInfo? r = await client.DemoulasCommonApiEndpointsAppVersionInfoEndpointAsync(null);
            Logger.LogInformation("Connected to SMART build: {BuildNumber}, git-hash: {GitHash}", r.BuildNumber, r.ShortGitHash);
            Console.WriteLine($"Connected to SMART build: {r.BuildNumber} git-hash: {r.ShortGitHash}");
        }

        List<Outcome> outcomes = [];
        foreach (IActivity activity in activitiesToRun)
        {
            Stopwatch wholeActivityStopWatch = Stopwatch.StartNew();
            Logger.LogInformation("------------------- Starting execution: {ActivityName}", activity.Name());

            Outcome outcome = await activity.Execute();
            outcomes.Add(outcome);

            string msg = outcome.Message.Replace("\n", "\n" + "   ").Trim();
            if (msg.Length != 0)
            {
                Logger.LogInformation("Activity message: {Message}", msg);
            }

            Logger.LogInformation("Activity {ActivityName} took {Duration}, Status: {Status}",
                activity.Name(), outcome.took, outcome.Status);

            // Console output: concise for success, detailed for errors
            if (outcome.Status == OutcomeStatus.Ok)
            {
                string timeStr = outcome.took.HasValue ? outcome.took.Value.ToString(@"mm\:ss") : "--:--";

                // Show EJR command for READY activities
                if (activity.Name().StartsWith('R') && !string.IsNullOrWhiteSpace(outcome.fullcommand))
                {
                    Console.WriteLine($"✓ {timeStr} {activity.Name()} (EJR {outcome.fullcommand})");
                }
                else
                {
                    Console.WriteLine($"✓ {timeStr} {activity.Name()}");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n------------------- ERROR in {activity.Name()} -------------------");
                Console.WriteLine($"Status: {outcome.Status}");
                Console.WriteLine($"Duration: {outcome.took}");
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    Console.WriteLine($"Message: {msg}");
                }
                if (!string.IsNullOrWhiteSpace(outcome.StandardOut))
                {
                    Console.WriteLine("\nOutput:");
                    Console.WriteLine(outcome.StandardOut);
                }
                if (!string.IsNullOrWhiteSpace(outcome.StandardError))
                {
                    Console.WriteLine("\nError Output:");
                    Console.WriteLine(outcome.StandardError);
                }
                Console.WriteLine("---------------------------------------------------------------");
                Console.ResetColor();
            }

            // Check for user-requested stop (non-blocking keyboard peek)
            // Only check if console input is not redirected
            try
            {
                if (!Console.IsInputRedirected && Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                    if (key.KeyChar is 's' or 'S')
                    {
                        Logger.LogWarning("User requested stop via 'S' key - stopping gracefully after current activity");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("User requested stop - exiting gracefully");
                        Console.ResetColor();
                        break;
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // Console input not available - skip keyboard check
            }

            if (outcome.Status == OutcomeStatus.Error)
            {
                Logger.LogError("Stopping execution due to error/failure: {ActivityName}", outcome.Name);
                CompletedWithoutError = false;
                break;
            }

            if (activity.Name().StartsWith('R'))
            {
                Match match = Regex.Match(outcome.StandardOut, @"LogFile:\s*(\S+)_(\S+)\.log");
                if (!match.Success)
                {
                    Logger.LogWarning("Unable to find READY LogFile in output for {ActivityNumber}", outcome.ActivityLetterNumber);
                }
                else
                {
                    string logFileId = match.Groups[2].Value;

                    List<(string, string)> reportFiles = ActivityToReports.GetReportFilenamesForActivity(activity.Name(), logFileId);
                    foreach ((string readyName, string smartName) in reportFiles)
                    {
                        GetGold.Fetch(ReadySshClientFactory.GetSftpClient(), readyName, $"{activity.Name()}-{smartName}");
                    }
                }
            }
        }

        string filePath = $"{DataDirectory}/outcome.json";
        JsonSerializerOptions options = new() { WriteIndented = true };
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(outcomes, options));
        Logger.LogInformation("Outcomes written to {FilePath}", filePath);

        TimeSpan wholeRunElapsed = wholeRunStopWatch.Elapsed;
        Logger.LogInformation("Completed YERunner. Took: {Hours}h {Minutes}m {Seconds}s",
            wholeRunElapsed.Hours, wholeRunElapsed.Minutes, wholeRunElapsed.Seconds);
        Console.WriteLine($"\nCompleted in {wholeRunElapsed.Hours}h {wholeRunElapsed.Minutes}m {wholeRunElapsed.Seconds}s");

        Process.Start("afplay", "/System/Library/Sounds/Submarine.aiff");
    }

    /// <summary>
    ///     Specify activities by enum to run. Uses injected ActivityFactory.
    /// </summary>
    protected List<IActivity> Specify(params ActivityName[] activityNames)
    {
        Dictionary<ActivityName, IActivity> activities = ActivityFactory.GetActivitiesByName();
        return activityNames.Select(name =>
        {
            if (!activities.TryGetValue(name, out IActivity? activity))
            {
                Logger.LogError("Activity '{ActivityName}' not found", name);
                throw new KeyNotFoundException($"Activity '{name}' not found");
            }

            return activity;
        }).ToList();
    }
}
