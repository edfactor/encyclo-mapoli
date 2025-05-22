using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace YEMatch;

public sealed class ActivityFactory
{
    // Ready activities which modify the system 
    //  private static readonly HashSet<string> activitiesWithUpdates = ["A6", "A7", "A12", "A13A", "A13B", "A18", "A21", "A22", "A23", "A24"]

    private static ActivityFactory? _instance;
    private readonly List<IActivity> _readyActivities;
    private readonly List<IActivity> _smartActivities;
    private readonly List<IActivity> _testActivities;
    private readonly List<IActivity> _parallelActivities =[];

    private ActivityFactory()
    {
        // used to get desired BaseDataDirectory for writing log files
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();

        string baseDir = config["BaseDataDirectory"] ?? Path.Combine("/tmp", "ye");
        Directory.CreateDirectory(baseDir);

        string dataDirectory = Path.Combine(baseDir, $"{DateTime.Now:dd-MMM-HH-mm}");
        Directory.CreateDirectory(dataDirectory);
        Console.WriteLine($"Directory created: file:///{dataDirectory}");

        Stopwatch wholeRunStopWatch = new();
        wholeRunStopWatch.Start();

        _smartActivities = SmartActivityFactory.CreateActivities(dataDirectory);
        _readyActivities = ReadyActivityFactory.CreateActivities(dataDirectory);
        _testActivities = TestActivityFactory.CreateActivities(dataDirectory);

        if (_readyActivities.Count != _smartActivities.Count)
        {
            throw new InvalidOperationException("READY and SMART activities are different length");
        }

        for (int i = 0; i < _readyActivities.Count; i++)
        {
            if (_smartActivities[i].Name().Substring(1) != _readyActivities[i].Name().Substring(1))
                // We always expect Ready short name to be the same as Smart short name (ie.  A7 and A7 should match.)
            {
                throw new InvalidOperationException(
                    $"READY and SMART activities are different at index {i}  s={_smartActivities[i].Name()} r={_readyActivities[i].Name()}");
            }

            _parallelActivities.Add(new ParallelActivity("P"+_readyActivities[i].Name().Substring(1),
                _readyActivities[i], _smartActivities[i]));
        }

    }

    private static ActivityFactory inst => _instance ??
                                           throw new InvalidOperationException("ActivityFactory not initialized.");

    public static void Initialize()
    {
        if (_instance is not null)
        {
            throw new InvalidOperationException("TestEnvironment already initialized.");
        }

        _instance = new ActivityFactory();
    }


    public static List<IActivity> AllActivties()
    {
        return inst._smartActivities.Concat(inst._readyActivities).Concat(inst._testActivities).ToList();
    }

    public static Dictionary<string, IActivity> AllActivtiesByName()
    {
        return inst._smartActivities
            .Concat(inst._readyActivities)
            .Concat(inst._testActivities)
            .Concat(inst._parallelActivities)
            .ToDictionary(s => s.Name(), a => a);
    }
}
