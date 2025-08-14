using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public sealed record CerficatePrintRequest : ProfitYearRequest
{
    public int[]? Ssns { get; set; }
    public int[]? BadgeNumbers { get; set; }
}
