using System.Diagnostics.CodeAnalysis;
using YEMatch.YEMatch.ArrangeActivites;
using YEMatch.YEMatch.AssertActivities;
using YEMatch.YEMatch.ReadyActivities;

namespace YEMatch.YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class SevenRun : Runnable
{
    /* Jusst run the smart side of the house - the seven steps to YE. */
    public override async Task Exec()
    {
        // YE Express though frozen to End for both READY and SMART
        await Run(Specify(
            "R0", // Start by importing the READY database from the scramble data.
            nameof(DropBadBenesReady), // Git rid of the two Bene/Employees w/o Demographics rows
            nameof(FixFrozenReady),
            nameof(ImportReadyDbToSmartDb), // Import SMART database from READY   database
            "S12", // Freeze on Smart
            "SanityCheckEmployeeAndBenes",
            "Give2023Hours",
            "S18_Rebuild2023ZeroCont",
            "S24_Rebuild2023Enrollment",
            nameof(UpdateNavigation),
            "S23", // Does Contributions
            "S24" // Create PAY450 report on READY, does enrollment update on SMART
        ));
    }
}
