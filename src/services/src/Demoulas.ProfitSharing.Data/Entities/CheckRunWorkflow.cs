namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// Tracks the workflow state for check run operations including FTP transfer, printing, and reconciliation.
/// Enforces business rule: Maximum 2 reprints allowed on same day as original CheckRunDate.
/// After midnight, reprints are no longer allowed for that run.
/// </summary>
public class CheckRunWorkflow
{
    /// <summary>
    /// Unique identifier for the workflow instance.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Profit-sharing year for this check run (e.g., 2024).
    /// </summary>
    public int ProfitYear { get; set; }

    /// <summary>
    /// Date checks are printed and distributed.
    /// Used to enforce same-day reprint constraint.
    /// </summary>
    public DateOnly CheckRunDate { get; set; }

    /// <summary>
    /// Current workflow step number.
    /// 1 = FTP transfer step
    /// 2 = Print checks step
    /// 3 = Reconciliation step
    /// </summary>
    public int StepNumber { get; set; }

    /// <summary>
    /// Current step status (Pending, InProgress, Completed, Failed).
    /// </summary>
    public CheckRunStepStatus StepStatus { get; set; }

    /// <summary>
    /// First check number in this run (5,000 - 10,000 random initialization).
    /// Nullable to support deferred initialization.
    /// </summary>
    public int? CheckNumber { get; set; }

    /// <summary>
    /// Number of times checks have been reprinted for this run.
    /// Defaults to 0.
    /// </summary>
    public int ReprintCount { get; set; } = 0;

    /// <summary>
    /// Maximum allowed reprints on same day (defaults to 2 per business requirements).
    /// Configurable for flexibility.
    /// </summary>
    public int MaxReprintCount { get; set; } = 2;

    /// <summary>
    /// User ID who created the workflow.
    /// </summary>
    public Guid CreatedByUserId { get; set; }

    /// <summary>
    /// Timestamp when the workflow was created.
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>
    /// User ID who last modified the workflow.
    /// Nullable until first modification.
    /// </summary>
    public Guid? ModifiedByUserId { get; set; }

    /// <summary>
    /// Timestamp of last modification.
    /// Nullable until first modification.
    /// </summary>
    public DateTimeOffset? ModifiedDate { get; set; }
}
