using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record FrozenProfitYearRequest : ProfitYearRequest
{
    public bool UseFrozenData { get; set; }
}
