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
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
#pragma warning disable DSM001
    public int? Ssn { get; set; }
#pragma warning restore DSM001
    public int? Percentage { get; set; }
    public short? BeneficiaryTypeId { get; set; }
}
