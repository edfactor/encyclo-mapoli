using System.Diagnostics.CodeAnalysis;
using YEMatch.YEMatch.ArrangeActivites;
using YEMatch.YEMatch.AssertActivities;
using YEMatch.YEMatch.SmartIntegrationTests;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace YEMatch.YEMatch.Runs;

/* A scratch pad for running different Activities on demand */

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class TinkerRun : Runnable
{
    public override async Task Exec()
    {
        await Run(Specify(
            "P0", // Initialize both databases in parallel
            nameof(DropBadBenesReady), // Fix bene in READY (already handled in SMART)
            "R3",  // Run and Gather PROF-TERM report
            "MasterInquiryDumper",
            nameof(IntTerminatedEmployee)
            ));
    }
}
