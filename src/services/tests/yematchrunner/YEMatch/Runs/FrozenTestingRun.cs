using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;
using static YEMatch.Activities.ActivityName;

namespace YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class FrozenTestingRun : Runnable
{
    public FrozenTestingRun(
        IActivityFactory activityFactory,
        IReadySshClientFactory readySshClientFactory,
        ISmartApiClientFactory smartApiClientFactory,
        ILogger<FrozenTestingRun> logger)
        : base(activityFactory, readySshClientFactory, smartApiClientFactory, logger)
    {
    }

    public override async Task Exec()
    {
        // Tests that Frozen is handled correctly by messing up the badges.
        await Run(Specify(
            R00_BuildDatabase, // import obfuscated
            TrimTo14Employees, // Reduces execution time to 1 minute
            ImportReadyDbToSmartDb,
            S12_ProfLoadYrEndDemoProfitShare, // Freeze on Smart
            OverwriteBadges, // Obliterate the Live Badges
            P18_ProfitShareReportFinalRun, // PAY426 / YearEndService
            TestPayProfitSelectedColumns, // Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate
            R21_ProfitShareUpdate, // PAY444 - update intermediate values
            R22_ProfitShareEdit, // PAY447 - creates a data file
            P23_ProfitMasterUpdate, // Does Contributions <-- smart does not yet use frozen - so it will fail
            TestProfitDetailSelectedColumns, // TEST: code,cont,earn,fort,cmt,zercont
            TestEtvaNow, // Verify ETVA for 2025
            TestEtvaPrior // Verify correct ETVA for 2024
        ));
    }
}
