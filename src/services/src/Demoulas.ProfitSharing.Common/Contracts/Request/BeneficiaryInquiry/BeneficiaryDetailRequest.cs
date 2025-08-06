using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
public record BeneficiaryDetailRequest
{
    public int BadgeNumber { get; set; }
    public int? Psn { get; set; }
}
