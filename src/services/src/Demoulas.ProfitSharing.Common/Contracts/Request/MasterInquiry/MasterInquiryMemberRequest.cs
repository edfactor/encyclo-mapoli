
namespace Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
public record MasterInquiryMemberRequest : ProfitYearRequest 
{
    public byte? MemberType { get; set; }
#pragma warning disable DSM001
    public int Ssn { get; set; }
#pragma warning restore DSM001
}
