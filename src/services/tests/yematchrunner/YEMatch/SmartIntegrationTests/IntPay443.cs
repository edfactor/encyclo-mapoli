using System.Diagnostics;
using YEMatch.YEMatch.AssertActivities;

namespace YEMatch.YEMatch.SmartIntegrationTests;

/* Runs the Smart Integration test for PAY443. */

public class IntPay443 : BaseActivity
{
    public override async Task<Outcome> Execute()
    {
        var workingDir =
            "/Users/robertherrmann/prj/smart-profit-sharing/src/services/tests/Demoulas.ProfitSharing.IntegrationTests";

        var args =
            "dotnet test --filter FullyQualifiedName~Pay443Test";

        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = args,
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var sw = Stopwatch.StartNew();
        using var process = new Process { StartInfo = psi };

        process.Start();
        string stdout = await process.StandardOutput.ReadToEndAsync();
        string stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        sw.Stop();
        
        Console.WriteLine(stdout);

        var status = process.ExitCode == 0 ? OutcomeStatus.Ok : OutcomeStatus.Error;

        return new Outcome(
            ActivityLetterNumber: "PAY443",
            Name: "Integration Test PAY443",
            fullcommand: $"dotnet {args}",
            Status: status,
            Message: status == OutcomeStatus.Ok ? "Tests passed" : "Tests failed",
            took: sw.Elapsed,
            isSmart: false,
            StandardOut: stdout,
            StandardError: stderr
        );
    }
}
