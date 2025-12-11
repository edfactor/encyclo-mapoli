using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Validation;

/// <summary>
/// Comprehensive cross-reference validation results for Master Update (PAY444|PAY447).
/// Validates that all prerequisite report values match their archived checksums.
/// </summary>
public record MasterUpdateCrossReferenceValidationResponse: ValidationResponse
{

    /// <summary>
    /// Whether the Master Update should be blocked due to validation failures
    /// </summary>
    public bool BlockMasterUpdate { get; init; }
}
