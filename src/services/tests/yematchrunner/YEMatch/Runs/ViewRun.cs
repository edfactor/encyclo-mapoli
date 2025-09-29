using System.Diagnostics.CodeAnalysis;

namespace YEMatch.YEMatch.Runs;

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
