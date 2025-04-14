using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;
internal class ParticipantTotalBalanceDto
{
    internal int? Ssn { get; set; }
    internal decimal? Distributions { get; set; }
    internal decimal? Earnings { get; set; }
    internal decimal? EtvaForfeitures { get; set; }
    internal decimal? Contributions { get; set; }
    internal decimal? Forfeitures { get; set; }
    internal decimal? VestedEarnings { get; set; }
    internal decimal? Total
    {
        get
        {
            return Contributions + Earnings + EtvaForfeitures + Distributions + Forfeitures + VestedEarnings;
        }
    }
}
