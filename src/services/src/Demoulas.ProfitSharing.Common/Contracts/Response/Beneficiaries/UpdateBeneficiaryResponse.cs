using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
public sealed record UpdateBeneficiaryResponse : UpdateBeneficiaryContactResponse
{
    public int BeneficiaryContactId { get; set; }
    public char? KindId { get; set; }
    public string? Relationship { get; set; }
    public decimal Percentage { get; set; }
    public int DemographicId { get; set; }
    public int BadgeNumber { get; set; }
}
