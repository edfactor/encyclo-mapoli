using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
public sealed record DistributionRunReportRequest : SortedPaginationRequestDto
{
    public char[]? DistributionFrequencies { get; set; }

    public static DistributionRunReportRequest RequestExample()
    {
        return new DistributionRunReportRequest
        {
            DistributionFrequencies = new char[] { 'Q', 'M' },
            Skip = 0,
            Take = 50,
            SortBy = "BadgeNumber",
            IsSortDescending = false,
        };
    }
}
