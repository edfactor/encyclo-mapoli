using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IFrozenService
{
    Task<FrozenStateResponse> FreezeDemographics(short profitYear, DateTime asOfDateTime, CancellationToken cancellationToken = default);
    Task<PaginatedResponseDto<FrozenStateResponse>> GetFrozenDemographics(SortedPaginationRequestDto request, CancellationToken cancellationToken = default);
    Task<FrozenStateResponse> GetActiveFrozenDemographic(CancellationToken cancellationToken = default);
}
