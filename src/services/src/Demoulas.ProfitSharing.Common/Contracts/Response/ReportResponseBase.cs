using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public sealed record ReportResponseBase<TResponse> where TResponse : class
{
    public required string ReportName { get; set; }
    public required DateTimeOffset ReportDate { get; set; }

    public required ISet<TResponse> Results { get; set; }
}
