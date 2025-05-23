using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
public class BeneficiaryDto
{
    public required int Id { get; set; }
    public required short PsnSuffix { get; set; } // Suffix for hierarchy (1000, 2000, etc.)

    public required int BadgeNumber { get; set; }
    public required int DemographicId { get; set; }

    public required BeneficiaryContactDto Contact { get; set; }

    public string? Relationship { get; set; }
    public char? KindId { get; set; }
    public BeneficiaryKindDto? Kind { get; set; }
    public required decimal Percent { get; set; }
}
