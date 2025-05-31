using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
public record MasterInquiryMemberRequest : SortedPaginationRequestDto 
{
    public int? SocialSecurity { get; set; }
    public int? BadgeNumber { get; set; }
    public short? PsnSuffix { get; set; }
}
