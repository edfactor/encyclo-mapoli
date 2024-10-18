using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record GetEligibleEmployeesResponse : ReportResponseBase<GetEligibleEmployeesResponseDto>
{ 
    public required int NumberReadOnFrozen { get; set; }
    public required int NumberNotSelected { get; set; }
    public required int NumberWritten { get; set; }
}
