using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Adjustments;
[NoMemberDataExposed]
public class MergeProfitDetailsResponse
{
    public int SourceSsn { get; set; }
    public int DestinationSsn { get; set; }
}
