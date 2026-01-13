using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;

public record BeneficiaryRequestDto : SortedPaginationRequestDto
{
    public int? BadgeNumber { get; set; }
    public int? PsnSuffix { get; set; }

    public static BeneficiaryRequestDto RequestExample()
    {
        return new BeneficiaryRequestDto
        {
            BadgeNumber = 1001,
            PsnSuffix = 1,
            Skip = 0,
            Take = 50
        };
    }
}
