using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record ProfitShareUpdateResponse : ReportResponseBase<MemberFinancialsResponse>
{
    public required bool IsReRunRequired { get; set; }
}
