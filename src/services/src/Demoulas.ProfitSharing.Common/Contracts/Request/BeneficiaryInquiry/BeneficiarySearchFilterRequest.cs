using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.BeneficiaryInquiry;
public record BeneficiarySearchFilterRequest:SortedPaginationRequestDto
{
    public int? BadgeNumber { get; set; }
    public int? Psn { get; set; }
    public string? Name { get; set; }
    public string? Ssn { get; set; }
    public required string MemberType { get; set; }//employee, beneficiary
}

