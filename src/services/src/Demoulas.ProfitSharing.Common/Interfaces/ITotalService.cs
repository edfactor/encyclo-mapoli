using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface ITotalService
{
    Task<BalanceEndpointResponse?> GetVestingBalanceForSingleMember(SearchBy searchBy, int employeeIdOrSsn, short profitYear);
}
