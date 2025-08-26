using System.Diagnostics.CodeAnalysis;
using YEMatch.YEMatch;
using YEMatch.YEMatch.ReadyActivities;
using YEMatch.YEMatch.Runs;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace YEMatch;

/* A scratch pad for running different Activities on demand */

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class TinkerRun : Runnable
{
    public override async Task Exec()
    {
        await Run(Specify(
            "SanityCheckEmployeeAndBenes"
        ));

        // GetGold.Fetch(DataDirectory, ReadyActivityFactory.SftpClient!)
    }
}
