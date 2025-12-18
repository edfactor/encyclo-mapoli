namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request model for retrieving Profit Sharing Report validation data.
/// </summary>
public sealed record ProfitSharingReportValidationRequest : FrozenProfitYearRequest
{
    /// <summary>
    /// The report suffix (1-8) identifying the specific Profit Sharing Report.
    /// </summary>
    public string ReportSuffix { get; set; } = string.Empty;
}
