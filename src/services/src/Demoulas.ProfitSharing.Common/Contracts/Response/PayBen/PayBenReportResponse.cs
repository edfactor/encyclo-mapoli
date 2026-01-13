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

    public static PayBenReportResponse ResponseExample()
    {
        return new PayBenReportResponse
        {
            Ssn = "xxx-xx-1234",
            BeneficiaryFullName = "Doe, Jane",
            Psn = "B12345",
            BadgeNumber = 700123,
            DemographicFullName = "Smith, John",
            Percentage = 50.00m
        };
    }
}
