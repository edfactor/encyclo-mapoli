using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service for validating archived report data against caller-provided current values
/// by comparing field checksums to detect data drift or integrity issues.
/// </summary>
public interface IChecksumValidationService
{
    /// <summary>
    /// Validates specific fields of a report by comparing caller-provided values against archived checksums.
    /// The caller provides the current field values they're seeing, and this service compares them
    /// against the archived checksums to detect if data has drifted.
    /// </summary>
    /// <param name="profitYear">The profit year to validate</param>
    /// <param name="reportType">The report type identifier (e.g., "PAY426N", "YearEndBreakdown")</param>
    /// <param name="fieldsToValidate">Dictionary of field names and their current values from caller's query.
    /// For example: { "TotalAmount": 12345.67m, "ParticipantCount": 100m }</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Result containing validation details:
    /// - IsValid: true if all provided fields match archived checksums
    /// - FieldResults: Per-field validation results showing which fields match/don't match
    /// - MismatchedFields: List of field names that don't match
    /// - Message: Description of validation result
    /// </returns>
    Task<Result<ChecksumValidationResponse>> ValidateReportFieldsAsync(
        short profitYear,
        string reportType,
        Dictionary<string, decimal> fieldsToValidate,
        CancellationToken cancellationToken = default);
}
