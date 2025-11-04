using System.Diagnostics.CodeAnalysis;
using YEMatch.YEMatch.ArrangeActivites;
using YEMatch.YEMatch.AssertActivities;
using YEMatch.YEMatch.SmartIntegrationTests;

namespace YEMatch.YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class GoldenExpressRun : Runnable
{
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
            "P0", // init both dbs
            nameof(DropBadBenesReady), // in READY, get rid of the two Bene/Employees w/o Demographics rows
            nameof(SanityCheckEmployeeAndBenes),
            
            // QPAY129 <-- should add this to validations
            "R13A", // PAYPROFIT-SHIFT
            "R13B", // PAYPROFIT-SHIFT
            "R14", // ZERO-PY-PD-PAYPROFIT

            "S12", // Freeze on Smart

                     // PAY426
            "R18",   // "PROF-SHARE sw[2]=1 CDATE=251227 YEAREND=Y" on READY    
                     // will set Earnpoints, fiddle with zerocont, clear new employee, clear certdate
            
            // build some
            // nameof(SmartPay456), // update payprofit[2025] PS_CERTIFICATE_ISSUED_DATE = null, EMPLOYEE_TYPE_ID = 0,  points_earned=0, zero_contribution_reason_id = 6?? 
            "S18" // Run YearEndService on SMART and
            
            
            // Should match
   //         nameof(TestPayProfitSelectedColumns) // VERIFY: Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate

#if false
            "R20", // PAY443    - Updates Earning points ?
            nameof(IntPay443), // Runs the SMART Integration test 
            "R21", // PAY444 - update intermediate values
            "R22", // PAY447 - creates a data file
            nameof(UpdateNavigation), // Update the navigation table
            "P23", // Does Contributions
            "TestProfitDetailSelectedColumns", // TEST: PROFIT_DETAILS; code,cont,earn,fort,cmt,zercont,enrollment_id
            "TestEtvaNow", // Verify ETVA for 2025
            "TestEtvaPrior", // Verify correct ETVA for 2024
            "P24", // Create PAY450 report on READY does enrollment update on SMART
            "P24B" // Updates the YEARS, and enrollment on READY, NOP on SMART
                
#endif
        ));
    }
}
