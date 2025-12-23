namespace Demoulas.ProfitSharing.Common.Contracts.Response.CheckRun;

/// <summary>
/// DTO for check run workflow information exposed through service APIs.
/// </summary>
public sealed class CheckRunWorkflowResponse
{
    /// <summary>
    /// Unique identifier for this workflow run.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The profit year this check run is for.
    /// </summary>
    public int ProfitYear { get; init; }

    /// <summary>
    /// The date this check run was initiated. Used to enforce same-day reprint constraints.
    /// </summary>
    public DateOnly CheckRunDate { get; init; }

    /// <summary>
    /// Current step number in the workflow (1-based).
    /// </summary>
    public int StepNumber { get; init; }

    /// <summary>
    /// Status of the current step.
    /// </summary>
    public CheckRunStepStatus StepStatus { get; init; }

    /// <summary>
    /// Starting check number for this run (randomly generated between 5,000-10,000).
    /// Null for partial steps (e.g., DJDE template generation only).
    /// </summary>
    public int? CheckNumber { get; init; }

    /// <summary>
    /// Number of times this run has been reprinted (max 2 per business rules).
    /// </summary>
    public int ReprintCount { get; init; }

    /// <summary>
    /// Maximum allowed reprints for this run (default 2).
    /// </summary>
    public int MaxReprintCount { get; init; }

    /// <summary>
    /// User who created this workflow run.
    /// </summary>
    public Guid CreatedByUserId { get; init; }

    /// <summary>
    /// Timestamp when this workflow run was created.
    /// </summary>
    public DateTimeOffset CreatedDate { get; init; }

    /// <summary>
    /// User who last modified this workflow run (step updates, reprint increments).
    /// </summary>
    public Guid? ModifiedByUserId { get; init; }

    /// <summary>
    /// Timestamp when this workflow run was last modified.
    /// </summary>
    public DateTimeOffset? ModifiedDate { get; init; }
}
