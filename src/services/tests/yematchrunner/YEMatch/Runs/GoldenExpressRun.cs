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
        // ideally as quickly as possible
        await Run(Specify(
            P00_BuildDatabase, // init both dbs
            DropBadBenesReady, // in READY, get rid of the two Bene/Employees w/o Demographics rows
            ActivityName.SanityCheckEmployeeAndBenes,
            R08_ProfitShareReport,
            IntPay426,
            IntPay426N,
            IntPay426N9,
            R13A_PayProfitShiftPartTime, // PAYPROFIT-SHIFT
            R13B_PayProfitShiftWeekly, // PAYPROFIT-SHIFT
            R14_ZeroPyPdPayProfit, // ZERO-PY-PD-PAYPROFIT
            S12_ProfLoadYrEndDemoProfitShare, // Freeze on Smart

            // PAY426
            R18_ProfitShareReportFinalRun, // "PROF-SHARE sw[2]=1 CDATE=251227 YEAREND=Y" on READY
            // will set Earnpoints, fiddle with zerocont, clear new employee, clear certdate
            IntTestPay426DataUpdates,
            S18_ProfitShareReportFinalRun, // Run YearEndService on SMART  - update EarnPoints

            // Should match
            ActivityName.TestPayProfitSelectedColumns, // VERIFY: Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate
            R20_ProfitForfeit, // PAY443
            IntPay443, // Runs the SMART Integration test
            R21_ProfitShareUpdate, // PAY444 - update intermediate values
            IntPay444Test, // <-- Will fail with Nava
            R22_ProfitShareEdit, // PAY447 - creates a data file
            IntPay447Test,
            UpdateNavigation, // Update the navigation table
            R23_ProfitMasterUpdate, // updates ready with contributions
            IntProfitMasterUpdateTest, // Runs Contributions on Smart

            // Ensure that YE update went to plan
            TestProfitDetailSelectedColumns, // TEST: PROFIT_DETAILS; code,cont,earn,fort,cmt,zercont,enrollment_id
            TestEtvaNow, // Verify ETVA for 2025
            TestEtvaPrior,
            R24_ProfPayMasterUpdate, // Create PAY450 report on READY
            R24B_ProfPayMasterUpdatePartTwo, // Updates the YEARS, and enrollment on READY, NOP on SMART
            S24_ProfPayMasterUpdate, // <--- Writes out update enrollments to the OPEN PROFIT YEAR
            IntPay450, // Does the FrozenService produce the same report as READY?
            TestEnrollmentComparison // Does READY's enrollments match SMART 2025 ?
        ));
    }
}
