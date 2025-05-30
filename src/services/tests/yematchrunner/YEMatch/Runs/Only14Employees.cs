using System.Diagnostics.CodeAnalysis;

namespace YEMatch;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class Only14Employees : Runnable
{
    public override async Task Exec()
    {
        await Run(Specify(
            "R0", // import obfuscated
            "TrimTo14Employees", // Reduces execution time to 1 minute
            "R15",
            "R16",
            "R17",
            "R18",
            "R19",
            "R20",
            "R21"
        ));
    }
}
