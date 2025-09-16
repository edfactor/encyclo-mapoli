using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IDistributionService
{
    Task<PaginatedResponseDto<DistributionSearchResponse>> SearchAsync(DistributionSearchRequest request, CancellationToken cancellationToken);
}
