using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public class SetExecutiveHoursAndDollarsRequest
{
    public required short ProfitYear { get; set; }
    public required List<SetExecutiveHoursAndDollarsDto> ExecutiveHoursAndDollars { get; set; } = new();
}

