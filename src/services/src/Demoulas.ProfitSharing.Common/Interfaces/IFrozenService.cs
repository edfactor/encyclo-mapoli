using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IFrozenService
{
    Task<FrozenStateResponse> FreezeDemographicsAsync(short profitYear, DateTime asOfDateTime, string? userName = "Unknown", CancellationToken cancellationToken = default);
    Task<PaginatedResponseDto<FrozenStateResponse>> GetFrozenDemographicsAsync(SortedPaginationRequestDto request, CancellationToken cancellationToken = default);
    Task<FrozenStateResponse> GetActiveFrozenDemographicAsync(CancellationToken cancellationToken = default);
}
