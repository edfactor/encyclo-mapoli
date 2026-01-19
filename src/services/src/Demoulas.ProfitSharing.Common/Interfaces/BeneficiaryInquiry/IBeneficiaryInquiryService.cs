using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;

namespace Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;

public interface IBeneficiaryInquiryService
{
    Task<PaginatedResponseDto<BeneficiarySearchFilterResponse>> BeneficiarySearchFilterAsync(BeneficiarySearchFilterRequest request, CancellationToken cancellationToken);
    Task<BeneficiaryResponse> GetBeneficiaryAsync(BeneficiaryRequestDto request, CancellationToken cancellationToken);
    Task<BeneficiaryTypesResponseDto> GetBeneficiaryTypesAsync(BeneficiaryTypesRequestDto beneficiaryTypesRequestDto, CancellationToken cancellationToken);
    Task<BeneficiaryDetailResponse> GetBeneficiaryDetailAsync(BeneficiaryDetailRequest request, CancellationToken cancellationToken);
}
