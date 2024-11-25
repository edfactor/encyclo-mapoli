using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Services;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface ITotalService
{
    Task<BalanceEndpointResponse?> GetVestingBalanceForSingleMember(SearchBy searchBy, string id, short profitYear);
}
