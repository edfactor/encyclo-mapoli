using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.PayBen;

public record PayBenReportResponse
{
    public string? Ssn { get; set; }
    [MaskSensitive] public string? BeneficiaryFullName { get; set; }
    public string? Psn { get; set; }
    public int? BadgeNumber { get; set; }
    [MaskSensitive] public string? DemographicFullName { get; set; }
    public decimal? Percentage { get; set; }
}
