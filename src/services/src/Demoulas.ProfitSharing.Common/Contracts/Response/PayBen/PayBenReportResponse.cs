using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.PayBen;
public record PayBenReportResponse
{
    public string? Ssn { get; set; }
    public string? BeneficiaryFullName { get; set; }
    public string? Psn { get; set; }
    public int? Badge { get; set; }
    public string? DemographicFullName { get; set; }
    public decimal? Percentage { get; set; }
}
