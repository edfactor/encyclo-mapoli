using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record YearEndProfitSharingReportRequest:ProfitYearRequest
{
    public bool IsYearEnd { get; set; }
    public short? MinimumAgeInclusive { get; set; } = 18;
    public short? MaximumAgeInclusive { get; set; }
    public short? MinimumHoursInclusive { get; set; } = 1000;
    public short? MaximumHoursInclusive { get; set; }
    public bool IncludeActiveEmployees { get; set; } = true;
    public bool IncludeInactiveEmployees { get; set; } = true;
    public bool IncludeTerminatedEmployees { get; set; }
    public bool IncludeBeneficiaries { get; set; } = true;
    public bool IncludeEmployeesWithPriorProfitSharingAmounts { get; set; } = true;
    public bool IncludeEmployeesWithNoPriorProfitSharingAmounts { get; set; } = true;
}
