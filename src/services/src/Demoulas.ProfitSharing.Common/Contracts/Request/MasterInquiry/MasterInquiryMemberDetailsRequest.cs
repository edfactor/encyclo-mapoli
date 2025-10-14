using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
public record MasterInquiryMemberDetailsRequest : SortedPaginationRequestDto
{
    public required byte? MemberType { get; set; }
    public int? Id { get; set; }
    public short? ProfitYear { get; set; }
    public byte? MonthToDate { get; set; }

    public int? BadgeNumber { get; set; }
    public short? PsnSuffix { get; set; }
    public string? Ssn { get; set; }
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
}
