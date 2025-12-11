using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;

namespace Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;

public interface IBeneficiaryInquiryService
{
    Task<PaginatedResponseDto<BeneficiarySearchFilterResponse>> BeneficiarySearchFilter(BeneficiarySearchFilterRequest request, CancellationToken cancellationToken);
    Task<BeneficiaryResponse> GetBeneficiary(BeneficiaryRequestDto request, CancellationToken cancellationToken);
    Task<BeneficiaryTypesResponseDto> GetBeneficiaryTypes(BeneficiaryTypesRequestDto beneficiaryTypesRequestDto, CancellationToken cancellationToken);
    Task<BeneficiaryDetailResponse> GetBeneficiaryDetail(BeneficiaryDetailRequest request, CancellationToken cancellationToken);
}
