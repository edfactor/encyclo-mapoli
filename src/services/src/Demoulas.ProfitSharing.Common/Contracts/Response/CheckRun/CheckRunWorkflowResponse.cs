using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.CheckRun;

/// <summary>
/// DTO for check run workflow information exposed through service APIs.
/// </summary>
public sealed class CheckRunWorkflowResponse : IProfitYearRequest
{
    /// <summary>
    /// Unique identifier for this workflow run.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The profit year this check run is for.
    /// </summary>
    public short ProfitYear { get; init; }

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
    public string? CreatedByUserName { get; init; }

    /// <summary>
    /// Timestamp when this workflow run was created.
    /// </summary>
    public DateTimeOffset CreatedDate { get; init; }

    /// <summary>
    /// User who last modified this workflow run (step updates, reprint increments).
    /// </summary>
    public string? ModifiedByUserName { get; init; }

    /// <summary>
    /// Timestamp when this workflow run was last modified.
    /// </summary>
    public DateTimeOffset? ModifiedDate { get; init; }

    /// <summary>
    /// Provides an example response for Swagger/OpenAPI documentation.
    /// </summary>
    public static CheckRunWorkflowResponse ResponseExample() => new()
    {
        Id = Guid.Parse("12345678-1234-1234-1234-123456789012"),
        ProfitYear = 2024,
        CheckRunDate = new DateOnly(2024, 12, 15),
        StepNumber = 1,
        StepStatus = CheckRunStepStatus.Completed,
        CheckNumber = 7542,
        ReprintCount = 0,
        MaxReprintCount = 2,
        CreatedByUserName = "jdoe",
        CreatedDate = DateTimeOffset.UtcNow,
        ModifiedByUserName = null,
        ModifiedDate = null
    };
}
