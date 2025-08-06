using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
public record BeneficiarySearchFilterResponse
{
    public int Badge { get; set; }
    public int Psn { get; set; }
    public string? Name { get; set; }
    public string? Ssn { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public int? Age { get; set; }

}
