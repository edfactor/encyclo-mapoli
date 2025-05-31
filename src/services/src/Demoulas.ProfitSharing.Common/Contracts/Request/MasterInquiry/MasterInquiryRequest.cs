using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
public record MasterInquiryRequest : MasterInquiryMemberRequest
{
    [DefaultValue(2025)]
    public short? EndProfitYear { get; set; }
    public byte? StartProfitMonth { get; set; }
    public byte? EndProfitMonth { get; set; }
    public byte? ProfitCode { get; set; }
    public decimal? ContributionAmount { get; set; }
    public decimal? EarningsAmount { get; set; }
    public decimal? ForfeitureAmount { get; set; }
    public decimal? PaymentAmount { get; set; }
    public string? Name { get; set; }
    public byte? PaymentType { get; set; }
    public byte? MemberType { get; set; }
}
