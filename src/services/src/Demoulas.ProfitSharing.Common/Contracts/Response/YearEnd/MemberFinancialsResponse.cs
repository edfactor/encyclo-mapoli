using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public class MemberFinancialsResponse 
{
    public long Psn { get; set; }
    public string? Name { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal Distributions { get; set; }
    public decimal Military { get; set; }
    public decimal Xfer { get; set; }
    public decimal Pxfer { get; set; }
    public long EmployeeTypeId { get; set; }
    public decimal Contributions { get; set; }
    public decimal IncomingForfeitures { get; set; }
    public decimal Earnings { get; set; }
    public decimal SecondaryEarnings { get; set; }
    public decimal EndingBalance { get; set; }
}
