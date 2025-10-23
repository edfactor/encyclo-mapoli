using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
public sealed record ManualChecksWrittenResponse : DistributionsOnHoldResponse
{
    public int? CheckNumber { get; set; }
    public decimal GrossAmount { get; set; }    
    public decimal FederalTaxAmount { get; set; }
    public decimal StateTaxAmount { get; set; }
}
