using System.Diagnostics;
using YEMatch.YEMatch.AssertActivities;
using YEMatch.YEMatch.ReadyActivities;
using YEMatch.YEMatch.SmartActivities;
using YEMatch.YEMatch.SmartIntegrationTests;

namespace YEMatch.YEMatch.Activities;

public sealed class ActivityFactory
{
    // Ready activities which modify the system 
    //  private static readonly HashSet<string> activitiesWithUpdates = ["A6", "A7", "A12", "A13A", "A13B", "A18", "A21", "A22", "A23", "A24"]

    private static ActivityFactory? _instance;
    private readonly List<IActivity> _parallelActivities = [];
    private readonly List<IActivity> _readyActivities;
    private readonly List<IActivity> _smartActivities;
    private readonly List<IActivity> _testActivities;
    private readonly List<IActivity> _integrationActivities;

    public ActivityFactory(string dataDirectory)
    {
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

            _parallelActivities.Add(new ParallelActivity("P" + _readyActivities[i].Name().Substring(1),
                _readyActivities[i], _smartActivities[i]));
        }

        _integrationActivities = IntegrationTestFactory.CreateActivities(dataDirectory);
    }

    private static ActivityFactory inst => _instance ??
                                           throw new InvalidOperationException("ActivityFactory not initialized.");

    public static void Initialize(string dataDirectory)
    {
        if (_instance is not null)
        {
            throw new InvalidOperationException("TestEnvironment already initialized.");
        }

        _instance = new ActivityFactory(dataDirectory);
    }
    
    public static Dictionary<string, IActivity> AllActivtiesByName()
    {
        return inst._smartActivities
            .Concat(inst._readyActivities)
            .Concat(inst._testActivities)
            .Concat(inst._parallelActivities)
            .Concat(inst._integrationActivities)
            .ToDictionary(s => s.Name(), a => a);
    }

    public static void SetNewScramble()
    {
        // This is a hack to reach into a class and fiddle with it, but we are still thinking
        // about how to dynamicaly switch between "classic" and "new" scramble choices.
        ReadyActivity ra = (ReadyActivity) ActivityFactory.AllActivtiesByName()["R0"];
        ra.Args = "profitshare"; // lock this Runner to the latest schema.
    }

    public static bool isNewScramble()
    {
        ReadyActivity ra = (ReadyActivity) ActivityFactory.AllActivtiesByName()["R0"];
        return ra.Args == "profitshare";
    }
}
