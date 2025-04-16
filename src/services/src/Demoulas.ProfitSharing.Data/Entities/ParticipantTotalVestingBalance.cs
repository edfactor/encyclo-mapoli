using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Data.Entities;
public class ParticipantTotalVestingBalance
{
    public int Ssn { get; set; }
    public decimal? VestedBalance { get; set; }
    public decimal? VestingPercent { get; set; }
    public decimal? CurrentBalance { get; set; }
    public byte? YearsInPlan { get; set; }
}
