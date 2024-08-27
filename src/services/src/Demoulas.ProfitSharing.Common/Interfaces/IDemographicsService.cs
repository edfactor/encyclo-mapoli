using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IDemographicsService
{
    Task<ISet<DemographicResponseDto>?> AddDemographics(IEnumerable<DemographicsRequest> demographics, CancellationToken cancellationToken);
}
