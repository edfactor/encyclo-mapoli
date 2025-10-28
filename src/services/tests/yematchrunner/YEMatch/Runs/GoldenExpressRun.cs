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
            "P0",
            nameof(DropBadBenesReady), // Git rid of the two Bene/Employees w/o Demographics rows
            nameof(TestPayProfitSelectedColumns), // VERIFY: Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate
            // Import SMART database from READY   database
            "R13A", // PAYPROFIT-SHIFT
            "R13B", // PAYPROFIT-SHIFT
            nameof(TestPayProfitSelectedColumns), // VERIFY: Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate
            "R14", // ZERO-PY-PD-PAYPROFIT   <--- oh boy
            "S12", // Freeze on Smart
            nameof(SanityCheckEmployeeAndBenes),
            nameof(TestPayProfitSelectedColumns), // VERIFY: Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate
            "R18",   // "PROF-SHARE sw[2]=1 CDATE=251227 YEAREND=Y" on READY    
                     // will clear Earnpoints, fiddle with zerocont, clear new employee, clear certdate

            // Clear all
            nameof(SmartPay456), // in SMART 2025 payprofit clear 3 columns, fiddle with one. - as if we rolled the year.
            // build some
            "S18", // Run YearEndService on SMART and
            // Should match
            nameof(TestPayProfitSelectedColumns) // VERIFY: Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate

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
