using YEMatch.YEMatch.ArrangeActivites;
using YEMatch.YEMatch.AssertActivities;
using YEMatch.YEMatch.SmartIntegrationTests;

namespace YEMatch.YEMatch.Runs;

#pragma warning disable AsyncFixer01

public class AutoMatchRun : Runnable
{
    public override async Task Exec()
    {
        await Run(Specify(
            "P0", // Initialize both databases in parallel
            nameof(DropBadBenesReady), // Fix bene in READY (already handled in SMART)
            "R3", // Run and Gather PROF-TERM report
            nameof(IntTerminatedEmployee),  // <<------- Terminations is a match
            "R13A", // PAYPROFIT-SHIFT
            "R13B", // PAYPROFIT-SHIFT
            "R14", // ZERO-PY-PD-PAYPROFIT

            // PAY456 and PAY426, "PROF-SHARE sw[2]=1 CDATE=251227 YEAREND=Y",  set Earnpoints, fiddle with zerocont, clear new employee, clear certdate  
            "R18",
            "R20", // PAY443 - generate report
            "S12", // Freeze on Smart
            "S18", // PAY426 sets earning points
            nameof(TestPayProfitSelectedColumns), // VERIFY: Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate
            nameof(IntPay443) // Runs the SMART Integration test      <<--------   PAY443 is a match
        ));
    }
}
