using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;

namespace Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
public interface IBeneficiaryInquiryService
{
    Task<PaginatedResponseDto<BeneficiaryDto>> GetBeneficiary(BeneficiaryRequestDto request, CancellationToken cancellationToken);
}
