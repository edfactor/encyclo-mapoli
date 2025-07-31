using System.Diagnostics.CodeAnalysis;

namespace YEMatch.YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class FrozenTestingRun : Runnable
{
    public override async Task Exec()
    {
        // Tests that Frozen is handled correctly by messing up the badges.
        await Run(Specify(
            "R0", // import obfuscated
            "TrimTo14Employees", // Reduces execution time to 1 minute
            "ImportReadyDbToSmartDb",
            "S12", // Freeze on Smart
            "OverwriteBadges", // Obliterate the Live Badges
            "P18", // PAY426 / YearEndService
            "TestPayProfitSelectedColumns", // Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate 
            "R21", // PAY444 - update intermediate values
            "R22", // PAY447 - creates a data file
            "P23", // Does Contributions <-- smart does not yet use frozen - so it will fail
            "TestProfitDetailSelectedColumns", // TEST: code,cont,earn,fort,cmt,zercont
            "TestEtvaNow", // Verify ETVA for 2025
            "TestEtvaPrior" // Verify correct ETVA for 2024
        ));
    }
}
