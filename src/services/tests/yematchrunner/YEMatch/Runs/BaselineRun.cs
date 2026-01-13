using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;
using static YEMatch.Activities.ActivityName;

namespace YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class BaselineRun : Runnable
{
    public BaselineRun(
        IActivityFactory activityFactory,
        IReadySshClientFactory readySshClientFactory,
        ISmartApiClientFactory smartApiClientFactory,
        ILogger<BaselineRun> logger)
        : base(activityFactory, readySshClientFactory, smartApiClientFactory, logger)
    {
    }

    public override async Task Exec()
    {
        // Tests that Frozen is handled correctly by messing up the badges.
        await Run(Specify(
            R00_BuildDatabase, // Start by importing the READY database from the scramble data.
            DropBadBenesReady, // Git rid of the two Bene/Employees w/o Demographics rows
            ImportReadyDbToSmartDb // Import SMART database from READY database
        ));
    }
}
