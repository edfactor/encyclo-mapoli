namespace Demoulas.ProfitSharing.Common.Contracts.Request.Validation;

/// <summary>
/// Request to validate all archived reports for a specific profit year (or all years).
/// </summary>
public sealed record ValidateAllReportsRequest
{
    /// <summary>
    /// Optional profit year filter. If null, validates all archived reports across all years.
    /// </summary>
    public short? ProfitYear { get; init; }
}
