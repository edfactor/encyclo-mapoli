

namespace Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;

public record BeneficiaryDetailRequest
{
    public int BadgeNumber { get; set; }
    public short? PsnSuffix { get; set; }

    public static BeneficiaryDetailRequest RequestExample()
    {
        return new BeneficiaryDetailRequest
        {
            BadgeNumber = 1001,
            PsnSuffix = 1
        };
    }
}
