using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;
using static YEMatch.Activities.ActivityName;

namespace YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class MasterInquiryRun : Runnable
{
    public MasterInquiryRun(
        IActivityFactory activityFactory,
        IReadySshClientFactory readySshClientFactory,
        ISmartApiClientFactory smartApiClientFactory,
        ILogger<MasterInquiryRun> logger)
        : base(activityFactory, readySshClientFactory, smartApiClientFactory, logger)
    {
    }

    public override async Task Exec()
    {
        await Run(Specify(
#if false
            R00_BuildDatabase, // import obfuscated/scramble
            DropBadBenesReady,
            ImportReadyDbToSmartDb, // Import SMART Schema directly from READY Schema (with dropped Bad Bene)
#endif
            TestMasterInquiry
        ));
    }
}
