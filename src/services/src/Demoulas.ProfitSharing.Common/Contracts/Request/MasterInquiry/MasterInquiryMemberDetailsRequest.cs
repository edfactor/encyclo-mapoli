using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
public record MasterInquiryMemberDetailsRequest : SortedPaginationRequestDto 
{
    public required byte? MemberType { get; set; }
    public required int? Id { get; set; }
}
