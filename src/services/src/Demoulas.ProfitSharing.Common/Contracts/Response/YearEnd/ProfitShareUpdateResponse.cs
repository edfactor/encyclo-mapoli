using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record ProfitShareUpdateResponse : ReportResponseBase<ProfitShareUpdateMemberResponse>
{
    public required bool HasExceededMaximumContributions { get; set; }
}
