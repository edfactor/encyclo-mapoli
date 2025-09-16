using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
public sealed record DistributionFrequencyResponse
{
    [Key]
    public char Id { get; set; }

    public required string Name { get; set; }
}
