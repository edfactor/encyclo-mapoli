using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
public record DistributionsOnHoldResponse
{
    public required string Ssn { get; set; }
    public required string PayTo { get; set; }
    public required decimal CheckAmount { get; set; }
}
