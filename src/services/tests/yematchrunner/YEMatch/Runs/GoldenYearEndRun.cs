namespace YEMatch.YEMatch.Runs;

using System.Diagnostics.CodeAnalysis;


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable CS0162 // Unreachable code detected

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class GoldenYearEndRun : Runnable
{
    public override async Task Exec()
    {
        // The database needs to be in the right state for this to work.
        throw new NotImplementedException();

        // Generate the Golden files.  Run READY from Frozen to the YE Completed.
        await Run(Specify(
            "R0", // import obfuscated
                  // "R1",  - we cant run these (R1...R14) because the SHIFT has already been run on the Scramble
                  // "R2",
                  // "R3",
                  // "R4",
                  // "R5",
                  // "R6",
                  // "R7",
                  // "R8",
                  // "R9",
                  // "R10",
                  // "R11",
                  // "R12",
                  // "R13A",
                  // "R13B",
                  // "R14",
            "R15",
            "R16",
            "R17",
            "R18",
            "R19",
            "R20",
            "R21",
            "R22",
            "R23",
            "R24",
            "R24B",
            "R25",
            "R26",
            "R27",
            "R28"
        ));
    }
}
