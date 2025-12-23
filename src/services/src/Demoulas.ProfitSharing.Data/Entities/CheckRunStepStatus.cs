namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// Status of a check run workflow step.
/// </summary>
public enum CheckRunStepStatus
{
    /// <summary>
    /// Step is pending and not yet started.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Step is currently in progress.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Step completed successfully.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Step failed with errors.
    /// </summary>
    Failed = 3
}
