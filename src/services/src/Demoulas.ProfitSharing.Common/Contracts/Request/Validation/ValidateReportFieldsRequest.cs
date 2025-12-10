namespace Demoulas.ProfitSharing.Common.Contracts.Request.Validation;

/// <summary>
/// Request to validate specific fields of a report against archived checksums.
/// Enables caller-driven validation where API consumers provide the field values
/// they're currently seeing and want to validate against the archived report.
/// </summary>
public sealed class ValidateReportFieldsRequest
{
    /// <summary>
    /// The profit year of the report to validate.
    /// </summary>
    public short ProfitYear { get; set; }

    /// <summary>
    /// The type of report to validate (e.g., "YearEndBreakdown", "ExecutiveSummary").
    /// </summary>
    public required string ReportType { get; set; }

    /// <summary>
    /// Dictionary of field names and their current values to validate.
    /// Key: Field name as it appears in the archived report (e.g., "TotalAmount", "ParticipantCount")
    /// Value: Current decimal value that the caller wants to validate
    /// </summary>
    /// <example>
    /// <code>
    /// {
    ///     "TotalAmount": 12345.67,
    ///     "ParticipantCount": 100,
    ///     "AverageDistribution": 123.45
    /// }
    /// </code>
    /// </example>
    public required Dictionary<string, decimal> Fields { get; set; }
}
