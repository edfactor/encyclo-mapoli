using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services.ProfitShareUpdate;

/// <summary>
///     A summary of financial information about a Member
/// </summary>
public class MemberFinancials
{
    public MemberFinancials(EmployeeFinancials empl, ProfitDetailTotals profitDetailTotals, MemberTotals memberTotals)
    {
        Badge = empl.EmployeeId;
        Psn = empl.EmployeeId;
        Name = empl.Name;
        Ssn = empl.Ssn;
        Earnings = empl.Earnings + empl.EarningsOnEtva;
        SecondaryEarnings = empl.SecondaryEarnings + empl.SecondaryEtvaEarnings;
        CurrentAmount = empl.CurrentAmount;
        EmployeeTypeId = empl.EmployeeTypeId;
        ContributionPoints = empl.PointsEarned;

        Common(profitDetailTotals, memberTotals);
    }

    public MemberFinancials(BeneficiaryFinancials bene, ProfitDetailTotals profitDetailTotals, MemberTotals memberTotals)
    {
        Badge = 0;
        Name = bene.Name;
        Ssn = bene.Ssn;
        Psn = bene.Psn;
        CurrentAmount = bene.CurrentAmount;
        SecondaryEarnings = bene.SecondaryEarnings;
        Earnings = bene.Earnings;
        
        Trace.Assert(memberTotals.IncomingForfeitureAmount == 0);
        Common(profitDetailTotals, memberTotals);
    }

    private void Common(ProfitDetailTotals profitDetailTotals, MemberTotals memberTotals)
    {
        Military = profitDetailTotals.MilitaryTotal;
        Distributions = profitDetailTotals.DistributionsTotal;
        Caf = profitDetailTotals.ClassActionFundTotal > 0 ? profitDetailTotals.ClassActionFundTotal : 0;
        Xfer = profitDetailTotals.AllocationsTotal;
        Pxfer = profitDetailTotals.PaidAllocationsTotal;

        Contributions = memberTotals.ContributionAmount;
        Earnings = memberTotals.EarningsAmount;
        EarningPoints = memberTotals.EarnPoints;
        IncomingForfeitures = memberTotals.IncomingForfeitureAmount - profitDetailTotals.ForfeitsTotal;
        
    }

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

    public decimal EndingBalance =>
        CurrentAmount + Contributions +
        Xfer - Pxfer +
        Earnings + SecondaryEarnings +
        IncomingForfeitures + Military +
        Caf -
        Distributions;
    
    public bool IsAllZeros() => CurrentAmount == 0m &&
                             Distributions == 0m &&
                             Contributions == 0m &&
                             Xfer == 0m &&
                             Pxfer == 0m &&
                             Military == 0m &&
                             IncomingForfeitures == 0m &&
                             Earnings == 0m &&
                             SecondaryEarnings == 0m &&
                             EndingBalance == 0m;
}
