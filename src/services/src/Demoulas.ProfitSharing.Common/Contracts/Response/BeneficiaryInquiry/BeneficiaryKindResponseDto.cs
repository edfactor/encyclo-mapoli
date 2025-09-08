using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;

[NoMemberDataExposed]
public class BeneficiaryKindResponseDto
{
    public List<BeneficiaryKindDto>? BeneficiaryKindList { get; set; }
}
