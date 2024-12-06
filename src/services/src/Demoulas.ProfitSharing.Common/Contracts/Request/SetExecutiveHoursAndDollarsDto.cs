using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record SetExecutiveHoursAndDollarsDto
{
    public int EmployeeId { get; set; }
    public decimal ExecutiveHours { get; set; }
    public decimal ExecutiveDollars { get; set; }

    public static SetExecutiveHoursAndDollarsDto Example()
    {
        return new() { EmployeeId = 9999, ExecutiveDollars = 721, ExecutiveHours = 1001 };
    }
}
