using System.Diagnostics.CodeAnalysis;
using YEMatch.YEMatch.ArrangeActivites;
using YEMatch.YEMatch.AssertActivities;

namespace YEMatch.YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class MasterInquiryRun : Runnable
{
    public override async Task Exec()
    {
        await Run(Specify(
#if false
            "R0", // import obfuscated/scramble
            nameof(DropBadBenesReady),
            nameof(FixFrozenReady),
            "ImportReadyDbToSmartDb", // Import SMART Schema directly from READY Schema (with dropped Bad Bene)
            "Give2023Hours",
            "S18_Rebuild2023ZeroCont",
            "S24_Rebuild2023Enrollment", // We use S24 as a tool to rebuild 2023
            
#endif
            nameof(TestMasterInquiry)
        ));
    }
}
