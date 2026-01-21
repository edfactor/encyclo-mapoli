using Microsoft.Extensions.Logging;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;
using static YEMatch.Activities.ActivityName;

namespace YEMatch.Runs;

#pragma warning disable AsyncFixer01

public class AutoMatchRun : Runnable
{
    public AutoMatchRun(
        IActivityFactory activityFactory,
        IReadySshClientFactory readySshClientFactory,
        ISmartApiClientFactory smartApiClientFactory,
        ILogger<AutoMatchRun> logger)
        : base(activityFactory, readySshClientFactory, smartApiClientFactory, logger)
    {
    }

    public override async Task Exec()
    {
        await Run(Specify(
            P00_BuildDatabase, // Initialize both databases in parallel
            DropBadBenesReady, // Fix bene in READY (already handled in SMART)
            R03_ProfTermination, // PROF-TERM (QPAY066)
            IntTerminatedEmployee, // Validate that Terminations is a match
            R04_ProfShareLoanBalance, // QPAY129
            IntTestQPay129, // Validate SMART QPAY129
            R08_ProfitShareReport,
            IntPay426,
            IntPay426N,
            IntPay426N9,
            R13A_PayProfitShiftPartTime, // PAYPROFIT-SHIFT
            R13B_PayProfitShiftWeekly, // PAYPROFIT-SHIFT
            R14_ZeroPyPdPayProfit, // ZERO-PY-PD-PAYPROFIT
            R18_ProfitShareReportFinalRun, // PAY456 and PAY426, "PROF-SHARE sw[2]=1 CDATE=251227 YEAREND=Y",  set Earnpoints, fiddle with zerocont, clear new employee, clear certdate
            R20_ProfitForfeit, // PAY443 - generate report
            S12_ProfLoadYrEndDemoProfitShare, // Freeze on Smart
            IntTestPay426DataUpdates, // Verify PAY426's intended data updates match READY's R18
            S18_ProfitShareReportFinalRun, // PAY426 sets earning points
            TestPayProfitSelectedColumns, // VERIFY: Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate (this also checkes enrollment_id/has_forfeit vesting_schedule_id)
            IntPay443, // Runs the SMART Integration test      <<--------   PAY443 is a match
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
