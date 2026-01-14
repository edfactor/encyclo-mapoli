
namespace Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;

public record MasterInquiryMemberRequest : ProfitYearRequest
{
    public byte? MemberType { get; set; }
    public int Id { get; set; }
}
