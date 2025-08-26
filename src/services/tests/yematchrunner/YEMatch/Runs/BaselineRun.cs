using System.Diagnostics.CodeAnalysis;

namespace YEMatch.YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]

public class BaselineRun : Runnable
{
    public override async Task Exec()
    {
        // Tests that Frozen is handled correctly by messing up the badges.
        await Run(Specify(
            "R0", // Start by importing the READY database from the scramble data.
            "DropBadBenes", // Git rid of the two Bene/Employees w/o Demographics rows
            "DropBadEmployee",
            "FixFrozen",
            "ImportReadyDbToSmartDb" // Import SMART database from READY database
        ));
    }
}
