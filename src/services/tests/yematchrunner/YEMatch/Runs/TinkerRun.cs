using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace YEMatch;

/* A scratch pad for running different Activities on demand */

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class TinkerRun : Runnable
{
    public override async Task Exec()
    {
        await Run(Specify(
            // nameof(S18_Rebuild2023ZeroCont)
            //nameof(S18_Rebuild2023ZeroCont)
             "S18"
            //"S24"
            // nameof(TestPayProfitSelectedColumns)
            // "TestProfitDetailSelectedColumns"
            // "S24_Rebuild2023Enrollment"
        ));
    }
}
