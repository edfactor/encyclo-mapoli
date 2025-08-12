using System.Diagnostics.CodeAnalysis;
using YEMatch.YEMatch.Runs;

namespace YEMatch;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]

//
// This exists to test that the views in READY and SMART agree.
//
public class ViewRun : Runnable
{
    public override async Task Exec()
    {
        await Run(Specify(
            "TestViews"
        ));
    }
}
