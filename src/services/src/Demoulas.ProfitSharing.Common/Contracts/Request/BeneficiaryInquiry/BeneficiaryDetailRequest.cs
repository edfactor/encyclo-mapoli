

namespace Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
public record BeneficiaryDetailRequest
{
    public int BadgeNumber { get; set; }
    public int? Psn { get; set; }
}
