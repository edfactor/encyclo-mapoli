using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record MasterInquiryRequest : PaginationRequestDto
{
    public int? StartProfitYear { get; set; }
    public int? EndProfitYear { get; set; }
    public byte? StartProfitMonth { get; set; }
    public byte? EndProfitMonth { get; set; }
    public string? ProfitCode { get; set; }
    public decimal? ContributionAmount { get; set; }
    public decimal? EarningsAmount { get; set; }
    public decimal? ForfeitureAmount { get; set; }
    public decimal? PaymentAmount { get; set; }
    public string? SocialSecurity { get; set; }
    public string? Comment { get; set; }
}
