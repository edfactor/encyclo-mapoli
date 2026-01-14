using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service for orchestrating cross-reference validation across multiple report fields and validation groups.
/// Coordinates validation of PAY443/PAY444 relationships and ensures data integrity across reports.
/// </summary>
/// <remarks>
/// PS-1721: Extracted from ChecksumValidationService to improve separation of concerns.
/// This service coordinates validation groups and orchestrates cross-reference checks.
/// </remarks>
public interface ICrossReferenceValidationService
{
    /// <summary>
    /// Performs comprehensive cross-reference validation for Master Update (PAY444|PAY447).
    /// Validates all prerequisite report values from PAY443, QPAY129, QPAY066TA, etc.
    /// against their archived checksums to ensure data integrity before Master Update execution.
    /// </summary>
    /// <param name="profitYear">The profit year to validate</param>
    /// <param name="currentValues">Dictionary of current field values to validate.
    /// Keys should be in format "ReportCode.FieldName" (e.g., "PAY443.DistributionTotals", "QPAY129.Distributions")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Result containing comprehensive cross-reference validation grouped by category
    /// (Distributions, Forfeitures, Contributions, etc.) with per-field validation status
    /// </returns>
    Task<Result<MasterUpdateCrossReferenceValidationResponse>> ValidateMasterUpdateCrossReferencesAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        CancellationToken cancellationToken = default);

    Task<Result<ValidationResponse>> ValidateProfitSharingReport(short profitYear, string reportSuffix, bool isFrozen, CancellationToken cancellationToken = default);

    Task<ValidationResponse> ValidateForfeitureAndPointsReport(short profitYear, decimal distributionTotal, decimal forfeitTotal, CancellationToken cancellationToken = default);
}
