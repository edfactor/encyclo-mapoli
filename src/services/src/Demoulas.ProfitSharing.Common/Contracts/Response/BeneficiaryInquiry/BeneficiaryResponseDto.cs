using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
public record BeneficiaryResponseDto
{
    public List<BeneficiaryDto>? BeneficiaryList { get; set; }
}
