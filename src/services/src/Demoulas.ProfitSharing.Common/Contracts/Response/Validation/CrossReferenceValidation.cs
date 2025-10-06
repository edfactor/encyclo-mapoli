namespace Demoulas.ProfitSharing.Common.Contracts.Response.Validation;

/// <summary>
/// Represents the validation status for a specific field cross-reference between reports.
/// Example: PAY444.DISTRIB = PAY443.TotalDistributions
/// </summary>
public class CrossReferenceValidation
{
    /// <summary>
    /// The field name being validated (e.g., "DISTRIB", "TotalDistributions", "Distributions")
    /// </summary>
    public required string FieldName { get; init; }

    /// <summary>
    /// The report code this field comes from (e.g., "PAY443", "QPAY129")
    /// </summary>
    public required string ReportCode { get; init; }

    /// <summary>
    /// Whether this field's value matches the expected value
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// The current value of this field (for display purposes)
    /// </summary>
    public decimal? CurrentValue { get; init; }

    /// <summary>
    /// The archived/expected value this field should match
    /// </summary>
    public decimal? ExpectedValue { get; init; }

    /// <summary>
    /// The difference between current and expected (CurrentValue - ExpectedValue)
    /// </summary>
    public decimal? Variance { get; init; }

    /// <summary>
    /// Human-readable message about the validation status
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// When the source report was archived (for audit trail)
    /// </summary>
    public DateTimeOffset? ArchivedAt { get; init; }

    /// <summary>
    /// Additional context or notes about this validation
    /// </summary>
    public string? Notes { get; init; }
}
