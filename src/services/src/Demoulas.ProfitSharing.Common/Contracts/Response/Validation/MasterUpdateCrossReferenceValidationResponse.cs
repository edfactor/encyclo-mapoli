using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Validation;

/// <summary>
/// Comprehensive cross-reference validation results for Master Update (PAY444|PAY447).
/// Validates that all prerequisite report values match their archived checksums.
/// </summary>
public class MasterUpdateCrossReferenceValidationResponse: IProfitYearRequest
{
    /// <summary>
    /// The profit year being validated
    /// </summary>
    public short ProfitYear { get; set; }

    /// <summary>
    /// Whether all cross-reference validations passed
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Overall summary message
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Grouped validation results by category (Distributions, Forfeitures, Contributions, etc.)
    /// </summary>
    public List<CrossReferenceValidationGroup> ValidationGroups { get; init; } = new();

    /// <summary>
    /// Count of total validations performed
    /// </summary>
    public int TotalValidations { get; init; }

    /// <summary>
    /// Count of validations that passed
    /// </summary>
    public int PassedValidations { get; init; }

    /// <summary>
    /// Count of validations that failed
    /// </summary>
    public int FailedValidations { get; init; }

    /// <summary>
    /// List of all reports that were validated against
    /// </summary>
    public List<string> ValidatedReports { get; init; } = new();

    /// <summary>
    /// When this validation was performed
    /// </summary>
    public DateTimeOffset ValidatedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Whether the Master Update should be blocked due to validation failures
    /// </summary>
    public bool BlockMasterUpdate { get; init; }

    /// <summary>
    /// List of critical issues that must be resolved before proceeding
    /// </summary>
    public List<string> CriticalIssues { get; init; } = new();

    /// <summary>
    /// Additional warnings that don't block the operation but should be reviewed
    /// </summary>
    public List<string> Warnings { get; init; } = new();
}
