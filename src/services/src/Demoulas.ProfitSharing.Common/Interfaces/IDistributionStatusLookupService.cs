using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IDistributionStatusLookupService
{
    Task<ListResponseDto<DistributionStatusResponse>> GetDistributionStatusesAsync(CancellationToken cancellationToken = default);
}
