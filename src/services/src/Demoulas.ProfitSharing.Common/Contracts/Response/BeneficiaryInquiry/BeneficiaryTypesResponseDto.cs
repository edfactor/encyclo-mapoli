using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;

[NoMemberDataExposed]
public class BeneficiaryTypesResponseDto
{
    public List<BeneficiaryTypeDto>? BeneficiaryTypeList { get; set; }

}
