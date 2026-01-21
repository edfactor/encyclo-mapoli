using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;

namespace YEMatch.Runs;

/// <summary>
///     TextWriter that writes to both console and a file, capturing all output.
///     Transient lines (those using ANSI clear-line codes) are shown on console but not written to file.
/// </summary>
internal sealed class DualWriter : TextWriter
{
    private readonly TextWriter _consoleWriter;
    private readonly StreamWriter _fileWriter;

    public DualWriter(TextWriter consoleWriter, string filePath)
    {
        _consoleWriter = consoleWriter;
        _fileWriter = new StreamWriter(filePath, false, Encoding.UTF8) { AutoFlush = true };
    }

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value)
    {
        _consoleWriter.Write(value);
        // Don't write single chars to file - they're typically part of transient updates
    }

    public override void Write(string? value)
    {
        _consoleWriter.Write(value);
        // Don't write to file - Write() without newline is used for transient status updates
        // Only WriteLine() output goes to the file
    }

    public override void WriteLine(string? value)
    {
        _consoleWriter.WriteLine(value);
        if (value != null)
        {
            // Skip lines that are just clearing the console (transient updates)
            if (value.Contains("\x1b[2K") || string.IsNullOrWhiteSpace(StripAnsiCodes(value)))
            {
                return;
            }

            string cleanValue = StripAnsiCodes(value);
            _fileWriter.WriteLine(cleanValue);
        }
        else
        {
            _fileWriter.WriteLine();
        }
    }

    public override void WriteLine()
    {
        _consoleWriter.WriteLine();
        _fileWriter.WriteLine();
    }

    private static string StripAnsiCodes(string input)
    {
        // Strip ANSI escape codes (like \x1b[2K\r for line clearing)
        return Regex.Replace(input, @"\x1b\[[0-9;]*[a-zA-Z]|\r", "");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fileWriter.Dispose();
        }
        base.Dispose(disposing);
    }

    public override async ValueTask DisposeAsync()
    {
        await _fileWriter.DisposeAsync();
        await base.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}

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

    /// <summary>
    ///     Parse the expected total run time from the golden run.txt file.
    /// </summary>
    private static TimeSpan? GetExpectedRunTime()
    {
        string goldenRunTxt = Path.Combine(ReadyActivity.OptionalLocalResourceBase, "golden", "run.txt");
        if (!File.Exists(goldenRunTxt))
        {
            return null;
        }

        try
        {
            string content = File.ReadAllText(goldenRunTxt);
            // Parse "Completed in Xh Ym Zs" format
            Match match = Regex.Match(content, @"Completed in (\d+)h (\d+)m (\d+)s");
            if (match.Success)
            {
                int hours = int.Parse(match.Groups[1].Value);
                int minutes = int.Parse(match.Groups[2].Value);
                int seconds = int.Parse(match.Groups[3].Value);
                return new TimeSpan(hours, minutes, seconds);
            }
        }
        catch
        {
            // Ignore errors reading golden file
        }

        return null;
    }

    /// <summary>
    ///     Format elapsed time as MM:SS or HH:MM:SS if over an hour.
    /// </summary>
    private static string FormatElapsed(TimeSpan elapsed)
    {
        return elapsed.TotalHours >= 1
            ? $"{(int)elapsed.TotalHours}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}"
            : $"{(int)elapsed.TotalMinutes}:{elapsed.Seconds:D2}";
    }

    protected async Task Run(List<IActivity> activitiesToRun)
    {
        // Capture console output to run.txt
        string runTxtPath = Path.Combine(DataDirectory, "run.txt");
        TextWriter originalOut = Console.Out;
        await using DualWriter dualWriter = new(originalOut, runTxtPath);
        Console.SetOut(dualWriter);

        try
        {
            await RunInternal(activitiesToRun, runTxtPath);
        }
        finally
        {
            // Restore original console output
            Console.SetOut(originalOut);
        }
    }

    private async Task RunInternal(List<IActivity> activitiesToRun, string runTextPath)
    {
        Stopwatch wholeRunStopWatch = Stopwatch.StartNew();
        TimeSpan? expectedRunTime = GetExpectedRunTime();

        Logger.LogInformation("Starting run with {ActivityCount} activities", activitiesToRun.Count);
        Console.WriteLine($"Starting {this.GetType().Name} with {activitiesToRun.Count} activities. Press 'S' or 's' at any time to stop gracefully (no enter needed).");
        if (expectedRunTime.HasValue)
        {
            Console.WriteLine($"Expected run time: {FormatElapsed(expectedRunTime.Value)} (from previous golden run)");
        }
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

            // Start background task to update progress display every 15 seconds
            using CancellationTokenSource cts = new();
            string activityName = activity.Name();
            Task progressTask = Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(15), cts.Token).ConfigureAwait(false);
                    if (cts.Token.IsCancellationRequested)
                    {
                        break;
                    }

                    TimeSpan elapsed = wholeRunStopWatch.Elapsed;
                    string progressText = expectedRunTime.HasValue
                        ? $"Running (at {FormatElapsed(elapsed)} of expected {FormatElapsed(expectedRunTime.Value)}) --> {activityName} "
                        : $"Running (elapsed {FormatElapsed(elapsed)}) --> {activityName} ";

                    Console.Write($"\x1b[2K\r{progressText}");
                }
            }, cts.Token);

            // Show initial running message
            string initialText = expectedRunTime.HasValue
                ? $"Running (at {FormatElapsed(wholeRunStopWatch.Elapsed)} of expected {FormatElapsed(expectedRunTime.Value)}) --> {activityName} "
                : $"Running (elapsed {FormatElapsed(wholeRunStopWatch.Elapsed)}) --> {activityName} ";
            Console.Write(initialText);

            Outcome outcome = await activity.Execute();

            // Stop the progress update task
            await cts.CancelAsync();
            try { await progressTask; } catch (OperationCanceledException) { /* Expected when cancelling */ }

            Console.Write("\x1b[2K\r"); // Clears line.
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
                if (!string.IsNullOrWhiteSpace(outcome.fullcommand))
                {
                    Console.WriteLine($"✓ {timeStr} {activity.Name(),-12} ({outcome.fullcommand})");
                }
                else
                {
                    Console.WriteLine($"✓ {timeStr} {activity.Name()}");
                }
            }
            else if (outcome.Status == OutcomeStatus.NoOperation)
            {
                // NoOperation is not an error - display as success with "No Operation" indicator
                Console.WriteLine($"✓ --:-- {activity.Name(),-12} No Operation");
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

            // Skip log file processing for NoOperation activities
            if (activity.Name().StartsWith('R') && outcome.Status != OutcomeStatus.NoOperation)
            {
                string? logFileId = null;

                // Try primary pattern: LogFile: path_PID.log
                Match match = Regex.Match(outcome.StandardOut, @"LogFile:\s*(\S+)_(\S+)\.log");
                if (match.Success)
                {
                    logFileId = match.Groups[2].Value;
                }
                else
                {
                    // Fallback: Look for spool files like PVTSYSOUT/SOMETHING-PID.CSV or PVTSYSOUT/SOMETHING-PID
                    // This handles SQL scripts that spool directly without EJR logging
                    Match spoolMatch = Regex.Match(outcome.StandardOut, @"PVTSYSOUT/[A-Z0-9-]+-(\d{6,})(?:\.\w+)?");
                    if (spoolMatch.Success)
                    {
                        logFileId = spoolMatch.Groups[1].Value;
                    }
                }

                if (logFileId == null)
                {
                    Logger.LogWarning("Unable to find READY LogFile in output for {ActivityNumber}", outcome.ActivityLetterNumber);
                }
                else
                {
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

        Completed(runTextPath);

        Process.Start("afplay", "/System/Library/Sounds/Submarine.aiff");
    }
    
    /// <summary>
    ///     Called when the run completes. Override in derived classes for custom completion behavior.
    /// </summary>
    protected virtual void Completed(string runTextPath)
    {
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
