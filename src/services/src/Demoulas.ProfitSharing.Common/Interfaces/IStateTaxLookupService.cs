using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IStateTaxLookupService
{
    Task<StateTaxLookupResponse> LookupStateTaxRate(string state, CancellationToken cancellationToken = default);
}
