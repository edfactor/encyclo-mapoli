using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IFrozenService
{
    Task<SetFrozenStateResponse> FreezeDemographics(short profitYear, DateTime asOfDateTime, CancellationToken cancellationToken = default);
    Task<List<SetFrozenStateResponse>> GetFrozenDemographics(CancellationToken cancellationToken = default);
}
