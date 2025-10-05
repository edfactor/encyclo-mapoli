using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service for validating archived report data against current calculations
/// by comparing checksums to detect data drift or integrity issues.
/// </summary>
public interface IChecksumValidationService
{
    /// <summary>
    /// Validates an archived report by comparing its stored checksum with a fresh calculation.
    /// </summary>
    /// <param name="profitYear">The profit year to validate</param>
    /// <param name="reportType">The report type identifier (e.g., "PAY426N", "YearEndBreakdown")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Result containing validation details:
    /// - IsValid: true if checksums match
    /// - ArchivedChecksum: The stored checksum from archive
    /// - CurrentChecksum: The freshly calculated checksum
    /// - Message: Description of validation result
    /// </returns>
    Task<Result<ChecksumValidationResponse>> ValidateReportChecksumAsync(
        short profitYear,
        string reportType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates multiple archived reports in a batch operation.
    /// Useful for periodic data integrity checks across all archived reports.
    /// </summary>
    /// <param name="profitYear">The profit year to validate (optional - if null, validates all years)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Result containing list of validation results for each report found.
    /// </returns>
    Task<Result<List<ChecksumValidationResponse>>> ValidateAllReportsAsync(
        short? profitYear = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Response DTO for checksum validation operations
/// </summary>
public class ChecksumValidationResponse
{
    /// <summary>
    /// The profit year that was validated
    /// </summary>
    public short ProfitYear { get; init; }

    /// <summary>
    /// The report type identifier
    /// </summary>
    public required string ReportType { get; init; }

    /// <summary>
    /// Whether the validation passed (checksums match)
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// The checksum stored in the archive
    /// </summary>
    public string? ArchivedChecksum { get; init; }

    /// <summary>
    /// The checksum calculated from current data
    /// </summary>
    public string? CurrentChecksum { get; init; }

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
}
