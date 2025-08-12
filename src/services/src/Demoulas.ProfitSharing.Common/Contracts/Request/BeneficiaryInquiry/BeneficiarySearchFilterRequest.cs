
using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
public record BeneficiarySearchFilterRequest:SortedPaginationRequestDto
{
    public int? BadgeNumber { get; set; }
    public short? PsnSuffix { get; set; }
    public string? Name { get; set; }
    public string? Ssn { get; set; }
    public byte MemberType { get; set; }
}

