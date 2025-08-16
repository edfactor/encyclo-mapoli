using System.Diagnostics.CodeAnalysis;

namespace YEMatch.YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class MasterInquiryRun : Runnable
{
    public override async Task Exec()
    {
        await Run(Specify(
//            "R0", // import obfuscated/scramble
//            "DropBadBenes",
//            "DropBadEmployee",
 //           "ImportReadyDbToSmartDb", // Import SMART Schema directly from READY Schema (with dropped Bad Bene)
            "TestMasterInquiry"
        ));
    }
}
