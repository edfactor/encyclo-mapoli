using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;

public class BeneficiaryResponse
{
    public PaginatedResponseDto<BeneficiaryDto>? Beneficiaries { get; set; }
    public PaginatedResponseDto<BeneficiaryDto>? BeneficiaryOf { get; set; }

    public static BeneficiaryResponse ResponseExample() => new()
    {
        Beneficiaries = new PaginatedResponseDto<BeneficiaryDto>
        {
            Results = new List<BeneficiaryDto> { BeneficiaryDto.ResponseExample() }
        },
        BeneficiaryOf = new PaginatedResponseDto<BeneficiaryDto>
        {
            Results = new List<BeneficiaryDto> { BeneficiaryDto.ResponseExample() }
        }
    };
}
