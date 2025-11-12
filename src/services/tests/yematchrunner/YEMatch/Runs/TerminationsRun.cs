using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;
using static YEMatch.Activities.ActivityName;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class TerminationsRun : Runnable
{
    public TerminationsRun(
        IActivityFactory activityFactory,
        IReadySshClientFactory readySshClientFactory,
        ISmartApiClientFactory smartApiClientFactory,
        ILogger<TerminationsRun> logger)
        : base(activityFactory, readySshClientFactory, smartApiClientFactory, logger)
    {
    }

    public override async Task Exec()
    {
        await Run(Specify(
            P00_BuildDatabase, // Initialize both databases in parallel
            DropBadBenesReady, // Fix bene in READY (already handled in SMART)
            R03_ProfTermination, // Run and Gather PROF-TERM report
            IntTerminatedEmployee
        ));
    }
}
