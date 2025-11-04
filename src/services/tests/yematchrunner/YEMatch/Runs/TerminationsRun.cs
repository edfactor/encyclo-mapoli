using System.Diagnostics.CodeAnalysis;
using YEMatch.YEMatch.ArrangeActivites;
using YEMatch.YEMatch.SmartIntegrationTests;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace YEMatch.YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class TerminationsRun : Runnable
{
    public override async Task Exec()
    {
        await Run(Specify(
            "P0", // Initialize both databases in parallel
            nameof(DropBadBenesReady), // Fix bene in READY (already handled in SMART)
            "R3", // Run and Gather PROF-TERM report
            nameof(IntTerminatedEmployee)
        ));
    }
}
