using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IDemographicsService
{
    Task<ISet<DemographicsResponseDto>?> AddDemographics(IEnumerable<DemographicsRequestDto> demographics, CancellationToken cancellationToken);
}
