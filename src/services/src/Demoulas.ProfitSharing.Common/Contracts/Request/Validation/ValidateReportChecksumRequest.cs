namespace Demoulas.ProfitSharing.Common.Contracts.Request.Validation;

/// <summary>
/// Request to validate a single archived report's checksum against current data.
/// </summary>
public sealed record ValidateReportChecksumRequest
{
    /// <summary>
    /// The profit year to validate.
    /// </summary>
    public required short ProfitYear { get; init; }

    /// <summary>
    /// The report type identifier (e.g., "PAY426N", "YearEndBreakdown").
    /// </summary>
    public required string ReportType { get; init; }
}
