using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Services;
public class ParticipantTotalVestingBalanceDto
{
    public long Ssn { get; set; }
    public Decimal VestedBalance { get; set; }
    public Decimal TotalDistributions { get; set; } 
    public Decimal Etva { get; set; }
    public Decimal VestingPercent { get; set; }
    public Decimal CurrentBalance { get; set; }
}
