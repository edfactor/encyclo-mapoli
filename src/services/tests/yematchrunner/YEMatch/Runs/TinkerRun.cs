using System.Diagnostics.CodeAnalysis;
using YEMatch.YEMatch.ArrangeActivites;
using YEMatch.YEMatch.AssertActivities;
using YEMatch.YEMatch.SmartIntegrationTests;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace YEMatch.YEMatch.Runs;

/* A scratch pad for running different Activities on demand */

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class TinkerRun : Runnable
{
    public override async Task Exec()
    {
        await Run(Specify(
            "P0",
            nameof(DropBadBenesReady),
            "R8",

        nameof(IntPay426),
        nameof(IntPay426N),
        nameof(IntPay426N9)

            //
            //      nameof(IntPay426),
        // nameof(IntPay426N)
            
                  //   "P0",
           // "R8"
        // nameof(IntTestQPay129)

#if false
            "P0", // init both dbs
            nameof(DropBadBenesReady), // in READY, get rid of the three Bene/Employees w/o Demographics rows
            nameof(SanityCheckEmployeeAndBenes),
            // Should -> RUN/Verify QPAY129 ?
            
            // Handle SHIFT Hours/Dollars
            "R13A", // PAYPROFIT-SHIFT
            "R13B", // PAYPROFIT-SHIFT
            "R14", // ZERO-PY-PD-PAYPROFIT

            "R17", // create PAY426 report on READY

            "S12" // Freeze on Smart

                     // runs PAY456.cbl and PAY426.cbl 
            "R18",   // "PROF-SHARE sw[2]=1 CDATE=251227 YEAREND=Y" on READY    
                     // will set Earnpoints, fiddle with zerocont, clear new employee, clear certdate
            

            // build some
            "S18"  // Run YearEndService on SMART <-- Set EarnPoints, ZeroCont, New Employee, CertDate
                      "R20", // 443

        nameof(IntPay443),
        "S20", // 443
        nameof(IntPay443)

            // Should match
            nameof(TestPayProfitSelectedColumns) // VERIFY: Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate
#endif
            ));
    }
}
