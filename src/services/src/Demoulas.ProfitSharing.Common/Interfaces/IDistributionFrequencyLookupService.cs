using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IDistributionFrequencyLookupService
{
    Task<ListResponseDto<DistributionFrequencyResponse>> GetDistributionFrequenciesAsync(CancellationToken cancellationToken = default);
}
