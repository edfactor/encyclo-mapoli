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
            
            "R3", // PROF-TERM (QPAY066)
            nameof(IntTerminatedEmployee),  // Validate that Terminations is a match
            
            "R4", // QPAY129
            nameof(IntTestQPay129), // Validate SMART QPAY129
            
            "R8",
            nameof(IntPay426),
            nameof(IntPay426N),
            nameof(IntPay426N9),
            
            "R13A", // PAYPROFIT-SHIFT
            "R13B", // PAYPROFIT-SHIFT
            "R14", // ZERO-PY-PD-PAYPROFIT
            "R18", // PAY456 and PAY426, "PROF-SHARE sw[2]=1 CDATE=251227 YEAREND=Y",  set Earnpoints, fiddle with zerocont, clear new employee, clear certdate
            "R20", // PAY443 - generate report

            "S12", // Freeze on Smart
            nameof(IntTestPay426DataUpdates), // Verify PAY426's intended data updates match READY's R18 
            "S18", // PAY426 sets earning points
            nameof(TestPayProfitSelectedColumns), // VERIFY: Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate
            nameof(IntPay443) // Runs the SMART Integration test      <<--------   PAY443 is a match
        ));
    }
}
