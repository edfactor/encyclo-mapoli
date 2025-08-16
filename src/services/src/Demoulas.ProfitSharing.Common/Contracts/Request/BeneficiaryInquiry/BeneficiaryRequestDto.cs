using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
 public record BeneficiaryRequestDto: SortedPaginationRequestDto
{
    public int? BadgeNumber { get; set; }
    public int? PsnSuffix { get; set; }

}
