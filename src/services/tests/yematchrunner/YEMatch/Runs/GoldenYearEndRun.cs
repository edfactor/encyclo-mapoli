using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;
using static YEMatch.Activities.ActivityName;

namespace YEMatch.Runs;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable CS0162 // Unreachable code detected

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class GoldenYearEndRun : Runnable
{
    public GoldenYearEndRun(
        IActivityFactory activityFactory,
        IReadySshClientFactory readySshClientFactory,
        ISmartApiClientFactory smartApiClientFactory,
        ILogger<GoldenYearEndRun> logger)
        : base(activityFactory, readySshClientFactory, smartApiClientFactory, logger)
    {
    }

    public override async Task Exec()
    {
        // Generate the Golden files.  Run READY from Frozen to the YE Completed.
        await Run(Specify(
            R00_BuildDatabase, // import obfuscated
            // R01-R14 - we cant run these because the SHIFT has already been run on the Scramble
            R15_ProfitSharingYTDWagesExtract2,
            R16_ReadyScreen00809Second,
            R17_ProfitShareReportEditRun,
            R18_ProfitShareReportFinalRun, // Profit Share Report - Final Run
            R19_GetEligibleEmployees,
            R20_ProfitForfeit,
            R21_ProfitShareUpdate,
            R22_ProfitShareEdit,
            R23_ProfitMasterUpdate,
            R24_ProfPayMasterUpdate,
            R24B_ProfPayMasterUpdatePartTwo,
            R25_ProfShareReportByAge,
            R26_ProfShareGrossReport,
            R27_ProfShareByStore,
            R28_PrintProfitCerts
        ));
    }
}
