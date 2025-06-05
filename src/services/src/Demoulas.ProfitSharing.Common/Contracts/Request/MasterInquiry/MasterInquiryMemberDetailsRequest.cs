using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
public record MasterInquiryMemberDetailsRequest : SortedPaginationRequestDto 
{
    public byte? MemberType { get; set; }
    public int? Id { get; set; }
}
