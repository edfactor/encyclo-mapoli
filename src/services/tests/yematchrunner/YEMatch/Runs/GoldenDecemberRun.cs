using System.Diagnostics.CodeAnalysis;
using YEMatch.YEMatch.Activities;
using YEMatch.YEMatch.ArrangeActivites;
using YEMatch.YEMatch.ReadyActivities;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace YEMatch.YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class GoldenDecemberRun : Runnable
{
    public override async Task Exec()
    {
        ActivityFactory.SetNewScramble();

        GetGold.Purge();

        // Generate the Golden files.  Run READY through december flow
        await Run(Specify(
            "R0", // import obfuscated
            nameof(DropBadBenesReady),
            "R1", // "PROFSHARE-SSN-CLEANUP-RPTS"
            "R2", // TERM-REHIRE
            "R3", // PROF-TERM
            "R4", // QRY-PSLOAN
            "R5", // PROF-DOLLAR-EXEC-EXTRACT
            // "R6", // PAYPROFIT-CLEAR-EXEC
            "R7", // !Ready-Screen-008-09
            "R8", // PROF-SHARE
            "R9", // !YE-Oracle-Payroll-Processing
            "R10", // Load-Oracle-PAYPROFIT(weekly job)
            "R11", // PROF-DOLLAR-EXTRACT 
            "R12" //  PROF-LOAD-YREND-DEMO-PROFSHARE
            // "R13A", // PAYPROFIT-SHIFT
            // "R13B", // PAYPROFIT-SHIFT
            // "R14" // ZERO-PY-PD-PAYPROFIT
        ));
        
    }
}
