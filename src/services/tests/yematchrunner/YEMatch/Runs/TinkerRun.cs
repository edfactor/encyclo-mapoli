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

            // Test the Enrollment change issue.

            "R0", // import obfuscated
            nameof(DropBadBenesReady),
            nameof(FixFrozenReady),
            "R15",
            "R16",
            "R17",
            "R18",
            "R19",
            "R20",
            "R21",
            "R22",
            "R23",
            "R24",
            "R24B",
            "R25",
            "R26",
            "R27",
            "R28"
            ));

        // use sql to compare proftitshare2 vs tbherrmann for py_enrollment
    }
}
