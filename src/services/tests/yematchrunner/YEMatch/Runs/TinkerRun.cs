using System.Diagnostics.CodeAnalysis;
using YEMatch.YEMatch;
using YEMatch.YEMatch.ArrangeActivites;
using YEMatch.YEMatch.ReadyActivities;
using YEMatch.YEMatch.Runs;
using YEMatch.YEMatch.SmartIntegrationTests;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace YEMatch;

/* A scratch pad for running different Activities on demand */

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class TinkerRun : Runnable
{
    public override async Task Exec()
    {
        await Run(Specify(
            "R0",
            nameof(DropBadBenesReady),  // Jane does this in her script
            nameof(FixFrozenReady), // Jane does this in her script
            nameof(ImportReadyDbToSmartDb), // Import SMART database from READY   database
            "S12", // Freeze on Smart
            nameof(SanityCheckEmployeeAndBenes),
            "R20", // PAY443 
            nameof(IntPay443)
        ));

        // GetGold.Fetch(DataDirectory, ReadyActivityFactory.SftpClient!)
    }
}
