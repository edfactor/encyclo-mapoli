namespace Demoulas.ProfitSharing.Data.Entities.CheckRun;

/// <summary>
/// Tracks the workflow state of a check printing run, including step progression and reprint limits.
/// Enforces business rules: max 2 reprints per day, step sequence tracking, temporal constraints.
/// </summary>
public sealed class CheckRunWorkflow
{
    /// <summary>
    /// Unique identifier for this workflow run.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The profit year this check run is for.
    /// </summary>
    public int ProfitYear { get; set; }

    /// <summary>
    /// The date this check run was initiated. Used to enforce same-day reprint constraints.
    /// </summary>
    public DateOnly CheckRunDate { get; set; }

    /// <summary>
    /// Current step number in the workflow (1-based).
    /// </summary>
    public int StepNumber { get; set; }

    /// <summary>
    /// Status of the current step.
    /// </summary>
    public CheckRunStepStatus StepStatus { get; set; }

    /// <summary>
    /// Starting check number for this run (randomly generated between 5,000-10,000).
    /// Null for partial steps (e.g., DJDE template generation only).
    /// </summary>
    public int? CheckNumber { get; set; }

    /// <summary>
    /// Number of times this run has been reprinted (max 2 per business rules).
    /// </summary>
    public int ReprintCount { get; set; }

    /// <summary>
    /// Maximum allowed reprints for this run (default 2).
    /// </summary>
    public int MaxReprintCount { get; set; }

    /// <summary>
    /// User who created this workflow run.
    /// </summary>
    public Guid CreatedByUserId { get; set; }

    /// <summary>
    /// Timestamp when this workflow run was created.
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>
    /// User who last modified this workflow run (step updates, reprint increments).
    /// </summary>
    public Guid? ModifiedByUserId { get; set; }

    /// <summary>
    /// Timestamp when this workflow run was last modified.
    /// </summary>
    public DateTimeOffset? ModifiedDate { get; set; }
}
