using System.Diagnostics.CodeAnalysis;

namespace YEMatch;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class GoldenReadyOnlyRun : Runnable
{
    public override async Task Exec()
    {
        // Generate the Golden files.  Run READY from Frozen to the YE Completed.
        await Run(Specify(
            "R0", // import obfuscated
            "DropBadBenes",
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
            "R25",
            "R26",
            "R27",
            "R28"
        ));

        // Copy the golden files to the integration test Resources directory
        GetGold.Fetch(ReadyActivityFactory.SftpClient!);
    }
}
