using System.Diagnostics.CodeAnalysis;

namespace YEMatch;

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
        // YE Express though frozen to End for both READY and SMART
        await Run(Specify(
            "R0", // Start by importing the READY database from the scramble data.
            "DropBadBenes", // Git rid of the two Bene/Employees w/o Demographics rows
            "ImportReadyDbToSmartDb", // Import SMART database from READY database
            "S12", // Freeze on Smart
            "P18", // Run YearEndServce on SMART and "PROF-SHARE sw[2]=1 CDATE=250104 YEAREND=Y" on READY
            "TestPayProfitSelectedColumns", // VERIFY: Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate 
            "R21", // PAY444 - update intermediate values
            "R22", // PAY447 - creates a data file
            "P23", // Does Contributions
            "TestProfitDetailSelectedColumns", // TEST: PROFIT_DETAILS; code,cont,earn,fort,cmt,zercont
            "TestEtvaNow", // Verify ETVA for 2025
            "TestEtvaPrior", // Verify correct ETVA for 2024
            "P24" // This should update enrollments
        ));
    }
}
