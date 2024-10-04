using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public class SetExecutiveHoursAndDollarsDto
{
    public int BadgeNumber { get; set; }
    public decimal ExecutiveHours { get; set; }
    public decimal ExecutiveDollars { get; set; }
}
