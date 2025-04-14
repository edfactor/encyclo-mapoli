using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using YEMatch.YEMatch;

namespace YEMatch;

internal static class Program
{
    private static readonly HashSet<string> activitiesWithUpdates = ["A6", "A7", "A12", "A13A", "A13B", "A18", "A20", "A23", "A24"];

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

        // This block lets you skip steps when doing specific testing
        var skipBoth = 0;

        var startOnStep = ""; // step to start with, ie. "A10"
        var stopOnStep = ""; // step to start with, ie. "A10"

        var stopOnSmartSide = true;

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

        readyActivities = readyActivities.Where(a => a.ActivityLetterNumber is "A0" or "A3").ToList();
        smartActivities = smartActivities.Where(a => a.ActivityLetterNumber is "A0" or "A3").ToList();

        var bothReadyAndSmartActivities = readyActivities.Zip(smartActivities, (a, b) => new[] { a, b })
            .SelectMany(x => x)
            .ToList();

        List<Outcome> outcomes = [];
        foreach (var activity in bothReadyAndSmartActivities)
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

    public static string ModChar(Activity activity)
    {
        return activitiesWithUpdates.Contains(activity.ActivityLetterNumber) ? "*" : " ";
    }

    public static string ModChar(Outcome outcome)
    {
        return activitiesWithUpdates.Contains(outcome.ActivityLetterNumber) ? "*" : " ";
    }
}
