using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public sealed record DistributionsAndForfeituresRequest: ProfitYearRequest
{
    public int? StartMonth { get; set; } = 1;
    public int? EndMonth { get; set; } = 12;
    public bool? IncludeOutgoingForfeitures { get; set; } = true;

}
