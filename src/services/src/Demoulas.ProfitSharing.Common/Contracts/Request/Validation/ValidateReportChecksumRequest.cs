namespace Demoulas.ProfitSharing.Common.Contracts.Request.Validation;

/// <summary>
/// Request to validate a single archived report's checksum against current data.
/// </summary>
public sealed record ValidateReportChecksumRequest : YearRequest
{
    /// <summary>
    /// The report type identifier (e.g., "PAY426N", "YearEndBreakdown").
    /// </summary>
    public required string ReportType { get; init; }
}
