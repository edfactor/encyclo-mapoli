namespace Demoulas.ProfitSharing.Common.Contracts.Response.CheckRun;

/// <summary>
/// Represents the status of a step in the check run workflow.
/// </summary>
public enum CheckRunStepStatus
{
    /// <summary>
    /// Step is pending execution.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Step is currently being executed.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Step has completed successfully.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Step has failed.
    /// </summary>
    Failed = 3
}
