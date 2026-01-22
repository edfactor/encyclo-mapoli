using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;
using static YEMatch.Activities.ActivityName;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace YEMatch.Runs;

/* A scratch pad for running different Activities on demand */

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class TinkerRun : Runnable
{
    public TinkerRun(
        IActivityFactory activityFactory,
        IReadySshClientFactory readySshClientFactory,
        ISmartApiClientFactory smartApiClientFactory,
        ILogger<TinkerRun> logger)
        : base(activityFactory, readySshClientFactory, smartApiClientFactory, logger)
    {
    }

    public override async Task Exec()
    {
        await Run(Specify(
            S12_ProfLoadYrEndDemoProfitShare
        ));
    }
}
