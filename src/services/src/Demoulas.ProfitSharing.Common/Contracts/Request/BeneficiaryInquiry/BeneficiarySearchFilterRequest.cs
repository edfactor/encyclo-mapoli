
using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;

public record BeneficiarySearchFilterRequest : SortedPaginationRequestDto
{
    public int? BadgeNumber { get; set; }
    public short? PsnSuffix { get; set; }
    public string? Name { get; set; }
#pragma warning disable DSM001
    public int? Ssn { get; set; }
#pragma warning restore DSM001
    public byte MemberType { get; set; }
}

