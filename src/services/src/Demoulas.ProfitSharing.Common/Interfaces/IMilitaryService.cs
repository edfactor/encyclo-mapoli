using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Services.Military;

public interface IMilitaryService
{
    Task<PaginatedResponseDto<MasterInquiryResponseDto>> GetMilitaryServiceRecordAsync(MilitaryContributionRequest request, CancellationToken cancellationToken = default);
}
