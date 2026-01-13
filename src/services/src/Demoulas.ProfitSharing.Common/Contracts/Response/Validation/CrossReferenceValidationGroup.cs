namespace Demoulas.ProfitSharing.Common.Contracts.Response.Validation;

/// <summary>
/// Represents a group of related cross-reference validations.
/// Example: "Total Distributions" validation checks PAY443, QPAY129, QPAY066TA all match
/// </summary>
public class CrossReferenceValidationGroup
{
    /// <summary>
    /// The name of this validation group (e.g., "Total Distributions", "Total Forfeitures")
    /// </summary>
    public required string GroupName { get; init; }

    /// <summary>
    /// Description of what this validation group checks
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Whether all validations in this group passed
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// The individual cross-reference validations in this group
    /// </summary>
    public List<CrossReferenceValidation> Validations { get; init; } = new();

    /// <summary>
    /// Summary message for this validation group
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Priority level for this validation (Critical, High, Medium, Low)
    /// </summary>
    public string Priority { get; init; } = "High";

    /// <summary>
    /// The validation rule being enforced (e.g., "PAY444.DISTRIB = PAY443.TotalDistributions = QPAY129.Distributions")
    /// </summary>
    public string? ValidationRule { get; init; }

    public static CrossReferenceValidationGroup ResponseExample()
    {
        return new CrossReferenceValidationGroup
        {
            GroupName = "Total Distributions",
            Description = "Validates total distribution amounts across reports",
            IsValid = true,
            Validations = new List<CrossReferenceValidation>
            {
                CrossReferenceValidation.ResponseExample()
            },
            Summary = "All distribution validations passed",
            Priority = "High",
            ValidationRule = "PAY444.DISTRIB = PAY443.TotalDistributions = QPAY129.Distributions"
        };
    }
}
