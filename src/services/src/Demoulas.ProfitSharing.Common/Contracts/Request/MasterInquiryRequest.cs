using System.ComponentModel;
using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record MasterInquiryRequest : SortedPaginationRequestDto 
{
    [DefaultValue(2023)]
    public short? StartProfitYear { get; set; }
    [DefaultValue(2024)]
    public short? EndProfitYear { get; set; }
    public byte? StartProfitMonth { get; set; }
    public byte? EndProfitMonth { get; set; }
    public byte? ProfitCode { get; set; }
    public decimal? ContributionAmount { get; set; }
    public decimal? EarningsAmount { get; set; }
    public decimal? ForfeitureAmount { get; set; }
    public decimal? PaymentAmount { get; set; }
    public int? SocialSecurity { get; set; }
    public string? Comment { get; set; }
    public byte? PaymentType { get; set; }
    public byte? MemberType { get; set; }
    public int? BadgeNumber { get; set; }
    public short? PsnSuffix { get; set; }
}
