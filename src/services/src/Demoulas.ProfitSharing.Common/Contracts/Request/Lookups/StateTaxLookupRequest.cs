using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
public sealed record StateTaxLookupRequest
{
    public required string State { get; init; }
}
