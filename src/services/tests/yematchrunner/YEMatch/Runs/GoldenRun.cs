using System.Diagnostics.CodeAnalysis;
using YEMatch.YEMatch.ReadyActivities;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace YEMatch.YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class GoldenRun : Runnable
{
    public override async Task Exec()
    {
        GetGold.Purge();

        // Generate the Golden files.  Run READY from Frozen to the YE Completed.
        await Run(Specify(
            "R0", // import obfuscated
            "DropBadBenes",
            "DropBadEmployee",
            "FixFrozen",
            // "R1",
            // "R2",
            // "R3",
            // "R4",
            // "R5",
            // "R6",
            // "R7",
            // "R8",

            // "R9",
            // "R10",
            // "R11",
            // "R12",
            // "R13",
            // "R14",
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
        // Copy the golden files to the integration test Resources directory
        GetGold.Fetch(DataDirectory, ReadyActivityFactory.SftpClient!);
    }
}
