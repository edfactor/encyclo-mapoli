using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IMilitaryService
{
    Task<Result<MasterInquiryResponseDto>> CreateMilitaryServiceRecordAsync(CreateMilitaryContributionRequest req, CancellationToken cancellationToken = default);
    Task<Result<PaginatedResponseDto<MasterInquiryResponseDto>>> GetMilitaryServiceRecordAsync(MilitaryContributionRequest req, CancellationToken cancellationToken = default);
}
