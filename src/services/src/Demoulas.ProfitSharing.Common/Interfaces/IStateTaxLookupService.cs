using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IStateTaxLookupService
{
    Task<StateTaxLookupResponse> LookupStateTaxRate(string state, CancellationToken cancellationToken = default);
}
