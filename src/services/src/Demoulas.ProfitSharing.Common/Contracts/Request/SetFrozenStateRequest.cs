using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public sealed class SetFrozenStateRequest
{
    public short ProfitYear { get; set; }
    public DateTime AsOfDateTime { get; set; }
}
