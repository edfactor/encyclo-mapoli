using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;
using static YEMatch.Activities.ActivityName;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class GoldenDecemberRun : Runnable
{
    public GoldenDecemberRun(
        IActivityFactory activityFactory,
        IReadySshClientFactory readySshClientFactory,
        ISmartApiClientFactory smartApiClientFactory,
        ILogger<GoldenDecemberRun> logger)
        : base(activityFactory, readySshClientFactory, smartApiClientFactory, logger)
    {
    }

    public override async Task Exec()
    {
        ActivityFactory.SetNewScramble();

        // GetGold.Purge()

        // Generate the Golden files.  Run READY through december flow
        await Run(Specify(
            R00_BuildDatabase, // import obfuscated
            DropBadBenesReady,
            R01_CleanUpReports, // "PROFSHARE-SSN-CLEANUP-RPTS"
            R02_MilitaryAndRehire, // TERM-REHIRE
            R03_ProfTermination, // PROF-TERM
            R04_ProfShareLoanBalance, // QRY-PSLOAN
            R05_ExtractExecutiveHoursAndDollars // PROF-DOLLAR-EXEC-EXTRACT,
            // R06 PAYPROFIT-CLEAR-EXEC -- We dont clear them, as of 06-Nov-2025, in the "SQL copy..." scrips
            //ActivityName.R07_ReadyScreen00809, // !Ready-Screen-008-09
            //ActivityName.R08_ProfitShareReport // PROF-SHARE

            // R09 !YE-Oracle-Payroll-Processing
            // R10 Load-Oracle-PAYPROFIT(weekly job)
            // R11 PROF-DOLLAR-EXTRACT
            // R12 PROF-LOAD-YREND-DEMO-PROFSHARE
            // R13A PAYPROFIT-SHIFT
            // R13B PAYPROFIT-SHIFT
            // R14 ZERO-PY-PD-PAYPROFIT
        ));
    }
}
