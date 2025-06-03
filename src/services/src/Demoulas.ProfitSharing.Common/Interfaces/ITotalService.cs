using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface ITotalService
{
    Task<BalanceEndpointResponse?> GetVestingBalanceForSingleMemberAsync(SearchBy searchBy, int badgeNumberOrSsn, short profitYear,
        CancellationToken cancellationToken);

    Task<List<BalanceEndpointResponse>> GetVestingBalanceForMembersAsync(SearchBy searchBy,
        ISet<int> badgeNumberOrSsnCollection, short profitYear, CancellationToken cancellationToken);
}
