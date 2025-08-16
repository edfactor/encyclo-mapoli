using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
public class BeneficiaryResponse
{
    public PaginatedResponseDto<BeneficiaryDto>? Beneficiaries { get; set; }
    public PaginatedResponseDto<BeneficiaryDto>? BeneficiaryOf { get; set; }
}
