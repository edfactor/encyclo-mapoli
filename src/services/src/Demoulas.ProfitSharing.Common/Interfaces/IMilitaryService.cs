using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IMilitaryService
{
    Task<Result<MilitaryContributionResponse>> CreateMilitaryServiceRecordAsync(CreateMilitaryContributionRequest req, CancellationToken cancellationToken = default);
    Task<Result<PaginatedResponseDto<MilitaryContributionResponse>>> GetMilitaryServiceRecordAsync(MilitaryContributionRequest req, bool isArchiveRequest, CancellationToken cancellationToken = default);
}
