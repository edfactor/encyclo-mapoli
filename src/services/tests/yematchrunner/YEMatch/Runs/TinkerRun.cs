using System.Diagnostics.CodeAnalysis;
using YEMatch.YEMatch.ReadyActivities;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace YEMatch.YEMatch.Runs;

/* A scratch pad for running different Activities on demand */

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class TinkerRun : Runnable
{
    public override async Task Exec()
    {
        await Run(Specify(
            #if false
            "R0", // Start by importing the READY database from the scramble data.
            "DropBadBenes", // Git rid of the two Bene/Employees w/o Demographics rows
            "DropBadEmployee",
            "ImportReadyDbToSmartDb", // Import SMART database from READY   database
            
            "S12", // Freeze on Smart
 //           "Give2023Hours",
//            "S18_Rebuild2023ZeroCont",
//            "S24_Rebuild2023Enrollment",

            "P18", // Run YearEndServce on SMART and "PROF-SHARE sw[2]=1 CDATE=250104 YEAREND=Y" on READY
            "TestPayProfitSelectedColumns", // VERIFY: Test PayProfit Updates; EarnPoints, ZeroCont, New Employee, CertDate
#endif
            "R20" // Generate PAY443
        ));

        GetGold.Fetch(DataDirectory, ReadyActivityFactory.SftpClient!);
    }
}
