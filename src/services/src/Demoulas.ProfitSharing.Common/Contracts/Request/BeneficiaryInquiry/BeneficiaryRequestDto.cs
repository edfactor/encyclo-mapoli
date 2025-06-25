using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
 public record BeneficiaryRequestDto: SortedPaginationRequestDto
{
    public int? BadgeNumber { get; set; }
    public short? PsnSuffix { get; set; }

}
