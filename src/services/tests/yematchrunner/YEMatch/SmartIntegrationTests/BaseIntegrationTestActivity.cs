using System.Diagnostics;
using YEMatch.AssertActivities;

namespace YEMatch.SmartIntegrationTests;

/// <summary>
///     Base class for integration test activities that run dotnet test commands
///     against the SMART profit sharing integration test suite.
/// </summary>
public abstract class BaseIntegrationTestActivity : BaseActivity
{
    private readonly string _activityId;
    private readonly string _integrationTestPath;
    private readonly string _testFilter;

    protected BaseIntegrationTestActivity(
        string integrationTestPath,
        string activityId,
        string testFilter)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(integrationTestPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(activityId);
        ArgumentException.ThrowIfNullOrWhiteSpace(testFilter);

        _integrationTestPath = integrationTestPath;
        _activityId = activityId;
        _testFilter = testFilter;
    }

    public override async Task<Outcome> Execute()
    {
        string args = $"dotnet test --filter FullyQualifiedName~{_testFilter}";

        ProcessStartInfo psi = new()
        {
            FileName = "dotnet",
            Arguments = $"test --filter FullyQualifiedName~{_testFilter}",
            WorkingDirectory = _integrationTestPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Stopwatch sw = Stopwatch.StartNew();
        using Process process = new() { StartInfo = psi };

        process.Start();
        string stdout = await process.StandardOutput.ReadToEndAsync();
        string stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        sw.Stop();

        OutcomeStatus status = process.ExitCode == 0 ? OutcomeStatus.Ok : OutcomeStatus.Error;

        return new Outcome(
            _activityId,
            $"Integration Test {_activityId}",
            $"dotnet {args}",
            status,
            status == OutcomeStatus.Ok ? "Tests passed" : "Tests failed",
            sw.Elapsed,
            false,
            stdout,
            stderr
        );
    }
}
