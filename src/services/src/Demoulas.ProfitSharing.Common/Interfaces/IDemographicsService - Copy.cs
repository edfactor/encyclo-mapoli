using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IPayProfitService
{
    Task<PaginatedResponseDto<DemographicsResponseDto>?> GetAllProfits(PaginationRequestDto req, CancellationToken cancellationToken = default);
    Task<ISet<DemographicsResponseDto>?> AddProfit(IEnumerable<DemographicsRequestDto> demographics, CancellationToken cancellationToken);
}
