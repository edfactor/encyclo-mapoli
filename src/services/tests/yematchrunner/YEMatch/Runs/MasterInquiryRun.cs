using System.Diagnostics.CodeAnalysis;

namespace YEMatch;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class MasterInquiryRun : Runnable
{
    public override async Task Exec()
    {
        await Run(Specify(
            // "R0", // import obfuscated
            // "DropBadBenes",
            // "R2S", // Import SMART Schema directly from READY Schema (with dropped Bad Bene)
            "TestMasterInquiry"
        ));
    }
}
