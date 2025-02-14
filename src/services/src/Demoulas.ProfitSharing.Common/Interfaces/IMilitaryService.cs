using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IMilitaryService
{
    Task<PaginatedResponseDto<MasterInquiryResponseDto>> GetMilitaryServiceRecordAsync(MilitaryContributionRequest req, CancellationToken cancellationToken = default);
}
