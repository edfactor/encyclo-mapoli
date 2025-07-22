using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Extensions;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.PayBen;
public record PayBenReportResponse
{
    public int BeneficiarySsn { get; set; }
    public string MaskedSsn => BeneficiarySsn.MaskSsn();
    public string? BeneficiaryFullName { get; set; }
    public string? Psn { get; set; }
    public int? Badge { get; set; }
    public string? DemographicFullName { get; set; }
    public decimal? Percentage { get; set; }
}
