using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;

namespace Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
public interface IBeneficiaryService
{
    Task<List<BeneficiaryDto>> GetBeneficiary(BeneficiaryRequestDto request, CancellationToken cancellationToken);
}
