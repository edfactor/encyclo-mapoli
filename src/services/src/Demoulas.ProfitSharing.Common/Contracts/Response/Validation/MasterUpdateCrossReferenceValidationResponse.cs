namespace Demoulas.ProfitSharing.Common.Contracts.Response.Validation;

/// <summary>
/// Comprehensive cross-reference validation results for Master Update (PAY444|PAY447).
/// Validates that all prerequisite report values match their archived checksums.
/// </summary>
public record MasterUpdateCrossReferenceValidationResponse : ValidationResponse
{

    /// <summary>
    /// Whether the Master Update should be blocked due to validation failures
    /// </summary>
    public bool BlockMasterUpdate { get; init; }

    public static new MasterUpdateCrossReferenceValidationResponse ResponseExample()
    {
        return new MasterUpdateCrossReferenceValidationResponse
        {
            ProfitYear = 2024,
            IsValid = true,
            Message = "Master Update validation passed - all prerequisites met",
            ValidationGroups = new List<CrossReferenceValidationGroup>
            {
                CrossReferenceValidationGroup.ResponseExample()
            },
            TotalValidations = 15,
            PassedValidations = 15,
            FailedValidations = 0,
            ValidatedReports = new List<string> { "PAY443", "QPAY129", "PAY444" },
            ValidatedAt = DateTimeOffset.UtcNow,
            CriticalIssues = new List<string>(),
            BlockMasterUpdate = false
        };
    }
}
