using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Services.ProfitShareUpdate;

/// <summary>
///     A summary of financial information about an Employee
/// </summary>
public record EmployeeFinancials
{
    public string? Name { get; set; }
    public long EmployeeId { get; set; }
    public int Ssn { get; set; }
    public long EnrolledId { get; set; }
    public long YearsInPlan { get; set; }
    public decimal CurrentAmount { get; set; }
    public long EmployeeTypeId { get; set; }
    public long PointsEarned { get; set; }
    public decimal Contributions { get; set; }
    public decimal IncomeForfeiture { get; set; }
    public decimal Earnings { get; set; }
    public decimal EtvaAfterVestingRules { get; set; } // Corresponds to PAYPROFIT.PY_PS_ETVA
    public decimal EarningsOnEtva { get; set; }
    public decimal SecondaryEarnings { get; set; }
    public decimal SecondaryEtvaEarnings { get; set; }
}
