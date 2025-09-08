using System.Diagnostics.CodeAnalysis;
using YEMatch.YEMatch.ArrangeActivites;
using YEMatch.YEMatch.ReadyActivities;
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
            "R0", // Start by importing the READY database from the scramble data.
            nameof(DropBadBenesReady), // Git rid of the two Bene/Employees w/o Demographics rows
            nameof(FixFrozenReady),
            nameof(ImportReadyDbToSmartDb), // Import SMART database from READY   database
            "S12", // Freeze on Smart
            "SanityCheckEmployeeAndBenes",
            "Give2023Hours",
            "S18_Rebuild2023ZeroCont",
            "S24_Rebuild2023Enrollment", // We use S24 as a tool to rebuild 2023
            "P18", // Run YearEndService on SMART and "PROF-SHARE sw[2]=1 CDATE=250104 YEAREND=Y" on READY
            "TestPayProfitSelectedColumns", // VERIFY: Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate
            "R20", // PAY443    - Updates Earning points ?
            nameof(IntPay443), // Runs the SMART Integration test 
            "R21", // PAY444 - update intermediate values
            "R22", // PAY447 - creates a data file
            "P23", // Does Contributions
            "TestProfitDetailSelectedColumns", // TEST: PROFIT_DETAILS; code,cont,earn,fort,cmt,zercont,enrollment_id
            "TestEtvaNow", // Verify ETVA for 2025
            "TestEtvaPrior", // Verify correct ETVA for 2024
            "P24", // Create PAY450 report on READY, does enrollment update on SMART
            "P24B" // Updates the YEARS, and enrollment on READY, NOP on SMART
        ));
        
    }
}
