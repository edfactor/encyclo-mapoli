using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Validation;

/// <summary>
/// Response DTO for checksum validation operations.
/// Contains per-field validation results and overall validation status.
/// </summary>
public class ChecksumValidationResponse : IProfitYearRequest
{
    /// <summary>
    /// The profit year that was validated
    /// </summary>
    public short ProfitYear { get; set; }

    /// <summary>
    /// The report type identifier
    /// </summary>
    public required string ReportType { get; init; }

    /// <summary>
    /// Whether the validation passed (all fields match archived checksums)
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Per-field validation results showing which fields match/don't match.
    /// Key: field name, Value: validation result for that field
    /// </summary>
    public Dictionary<string, FieldValidationResult> FieldResults { get; init; } = new();

    /// <summary>
    /// List of field names that did not match archived checksums
    /// </summary>
    public List<string> MismatchedFields { get; init; } = new();

    /// <summary>
    /// Descriptive message about the validation result
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// When the archived report was created
    /// </summary>
    public DateTimeOffset? ArchivedAt { get; init; }

    /// <summary>
    /// When the validation was performed
    /// </summary>
    public DateTimeOffset ValidatedAt { get; init; } = DateTimeOffset.UtcNow;

    public static ChecksumValidationResponse ResponseExample()
    {
        return new ChecksumValidationResponse
        {
            ProfitYear = 2024,
            ReportType = "PAY443",
            IsValid = true,
            FieldResults = new Dictionary<string, FieldValidationResult>
            {
                { "TotalDistributions", FieldValidationResult.ResponseExample() }
            },
            MismatchedFields = new List<string>(),
            Message = "All fields match archived checksums",
            ArchivedAt = DateTimeOffset.UtcNow.AddDays(-1),
            ValidatedAt = DateTimeOffset.UtcNow
        };
    }
}
