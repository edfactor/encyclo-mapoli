using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IFrozenService
{
    Task<FrozenStateResponse> FreezeDemographics(short profitYear, DateTime asOfDateTime, CancellationToken cancellationToken = default);
    Task<List<FrozenStateResponse>> GetFrozenDemographics(CancellationToken cancellationToken = default);
    Task<FrozenStateResponse> GetActiveFrozenDemographic(CancellationToken cancellationToken = default);
}
