using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;

namespace Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
public interface IBeneficiaryInquiryService
{
    Task<PaginatedResponseDto<BeneficiarySearchFilterResponse>> BeneficiarySearchFilter(BeneficiarySearchFilterRequest request, CancellationToken cancellation);
    Task<BeneficiaryResponse> GetBeneficiary(BeneficiaryRequestDto request, CancellationToken cancellationToken);
    Task<BeneficiaryTypesResponseDto> GetBeneficiaryTypes(BeneficiaryTypesRequestDto beneficiaryTypesRequestDto, CancellationToken cancellation);
    Task<BeneficiaryKindResponseDto> GetBeneficiaryKind(BeneficiaryKindRequestDto beneficiaryKindRequestDto, CancellationToken cancellation);
    Task<BeneficiaryDetailResponse> GetBeneficiaryDetail(BeneficiaryDetailRequest request, CancellationToken cancellationToken);
}
