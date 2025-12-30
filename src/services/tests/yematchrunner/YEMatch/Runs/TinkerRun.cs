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
            P00_BuildDatabase, // init both dbs
            DropBadBenesReady, // in READY, get rid of the two Bene/Employees w/o Demographics rows
            ActivityName.SanityCheckEmployeeAndBenes,
            R08_ProfitShareReport,
            IntPay426,
            IntPay426N,
            IntPay426N9
        /*
                    IntProfitMasterUpdateTest, // Runs Contributions on Smart

                    // Ensure that YE update went to plan
                    TestProfitDetailSelectedColumns, // TEST: PROFIT_DETAILS; code,cont,earn,fort,cmt,zercont,enrollment_id
                    TestEtvaNow, // Verify ETVA for 2025
                    TestEtvaPrior,

                    R24_ProfPayMasterUpdate, // Create PAY450 report on READY
                    // R24B_ProfPayMasterUpdatePartTwo // Updates the YEARS, and enrollment on READY, NOP on SMART

                    S24_ProfPayMasterUpdate, // <--- Writes out update enrollments to the OPEN PROFIT YEAR
                    IntPay450 // Does the FrozenService produce the same report as READY?

                    // S24_ProfPayMasterUpdate // <--- Writes out update enrollments
        */
        ));
    }
}
