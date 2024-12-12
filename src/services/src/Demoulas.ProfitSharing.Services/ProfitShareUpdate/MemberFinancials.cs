using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Services.ProfitShareUpdate;
/// <summary>
///     A summary of financial information about a Member
/// </summary>
public class MemberFinancials
{
    public long Badge { get; set; }
    public long Psn { get; set; }
    public string? Name { get; set; }
    public int Ssn { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal Distributions { get; set; }
    public decimal Military { get; set; }
    public decimal Xfer { get; set; }
    public decimal Pxfer { get; set; }
    public byte EmployeeTypeId { get; set; }
    public int ContributionPoints { get; set; }
    public int EarningPoints { get; set; }
    public decimal Contributions { get; set; }
    public decimal IncomingForfeitures { get; set; }
    public decimal Earnings { get; set; }
    public decimal SecondaryEarnings { get; set; }
    public decimal MaxOver { get; set; }
    public int MaxPoints { get; set; }
    public decimal Caf { get; set; }
    public decimal EndingBalance { get; set; }
}
