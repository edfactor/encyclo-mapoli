using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public class PayProfitBadgesNotInDemographicsResponse
{
    public required long EmployeeBadge { get; set; }
    public required long EmployeeSSN { get; set; }
}
