using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;
using static YEMatch.Activities.ActivityName;

namespace YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class GoldenExpressRun : Runnable
{
    public GoldenExpressRun(
        IActivityFactory activityFactory,
        IReadySshClientFactory readySshClientFactory,
        ISmartApiClientFactory smartApiClientFactory,
        ILogger<GoldenExpressRun> logger)
        : base(activityFactory, readySshClientFactory, smartApiClientFactory, logger)
    {
    }

    /*
     * The express run only runs activities which effect the database on READY and SMART.
     * So activities which are only reports are skipped.
     *
     * The scramble database has already had the payprofit shift.   The import creates the DEMO_PROFITSHARE table.
     * So both READY and SMART are at the beginning of the Frozen Process.
     */
    public override async Task Exec()
    {
        // YE Express runs though frozen to End for both READY and SMART
        await Run(Specify(
            P00_BuildDatabase, // init both dbs
            DropBadBenesReady, // in READY, get rid of the two Bene/Employees w/o Demographics rows
            SanityCheckEmployeeAndBenes,

            // QPAY129 <-- should add this to validations
            R13A_PayProfitShiftPartTime, // PAYPROFIT-SHIFT
            R13B_PayProfitShiftWeekly, // PAYPROFIT-SHIFT
            R14_ZeroPyPdPayProfit, // ZERO-PY-PD-PAYPROFIT
            S12_ProfLoadYrEndDemoProfitShare, // Freeze on Smart

            // PAY426
            R18_ProfitShareReportFinalRun, // "PROF-SHARE sw[2]=1 CDATE=251227 YEAREND=Y" on READY
            // will set Earnpoints, fiddle with zerocont, clear new employee, clear certdate

            // build some
            S18_ProfitShareReportFinalRun // Run YearEndService on SMART and

            // Should match
            //         TestPayProfitSelectedColumns // VERIFY: Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate

#if false
            R20_ProfitForfeit, // PAY443    - Updates Earning points ?
            IntPay443, // Runs the SMART Integration test
            R21_ProfitShareUpdate, // PAY444 - update intermediate values
            R22_ProfitShareEdit, // PAY447 - creates a data file
            UpdateNavigation, // Update the navigation table
            P23_ProfitMasterUpdate, // Does Contributions
            TestProfitDetailSelectedColumns, // TEST: PROFIT_DETAILS; code,cont,earn,fort,cmt,zercont,enrollment_id
            TestEtvaNow, // Verify ETVA for 2025
            TestEtvaPrior, // Verify correct ETVA for 2024
            P24_ProfPayMasterUpdate, // Create PAY450 report on READY does enrollment update on SMART
            P24B_ProfPayMasterUpdatePartTwo // Updates the YEARS, and enrollment on READY, NOP on SMART
#endif
        ));
    }
}
