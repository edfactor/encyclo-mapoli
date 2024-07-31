using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public class NamesMissingCommaResponse
{
    public required long EmployeeBadge { get; set; }
    public required long EmployeeSSN { get; set; }
    public required string EmployeeName { get; set; }
}
