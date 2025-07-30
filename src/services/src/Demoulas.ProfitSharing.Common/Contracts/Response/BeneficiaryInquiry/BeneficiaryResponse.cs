using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
public class BeneficiaryResponse
{
    public PaginatedResponseDto<BeneficiaryDto>? Beneficiaries { get; set; }
    public PaginatedResponseDto<BeneficiaryDto>? BeneficaryOf { get; set; }
}
