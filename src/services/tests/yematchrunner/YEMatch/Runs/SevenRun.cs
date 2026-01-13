using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;
using static YEMatch.Activities.ActivityName;

namespace YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class SevenRun : Runnable
{
    public SevenRun(
        IActivityFactory activityFactory,
        IReadySshClientFactory readySshClientFactory,
        ISmartApiClientFactory smartApiClientFactory,
        ILogger<SevenRun> logger)
        : base(activityFactory, readySshClientFactory, smartApiClientFactory, logger)
    {
    }

    /* Jusst run the smart side of the house - the seven steps to YE. */
    public override async Task Exec()
    {
        // YE Express though frozen to End for both READY and SMART
        await Run(Specify(
            R00_BuildDatabase, // Start by importing the READY database from the scramble data.
            DropBadBenesReady, // Git rid of the two Bene/Employees w/o Demographics rows
            ImportReadyDbToSmartDb, // Import SMART database from READY   database
            S12_ProfLoadYrEndDemoProfitShare, // Freeze on Smart
            SanityCheckEmployeeAndBenes,
            UpdateNavigation,
            S23_ProfitMasterUpdate, // Does Contributions
            S24_ProfPayMasterUpdate // Create PAY450 report on READY, does enrollment update on SMART
        ));
    }
}
