using System.Diagnostics;

namespace Demoulas.ProfitSharing.Services.ProfitShareUpdate;

/// <summary>
///     A summary of financial information about a Member
/// </summary>
public class MemberFinancials
{
    public MemberFinancials(EmployeeFinancials empl, ProfitDetailTotals profitDetailTotals, MemberTotals memberTotals)
    {
        IsEmployee = true;
        Badge = empl.BadgeNumber;
        Psn = empl.BadgeNumber;
        Name = empl.Name;
        Ssn = empl.Ssn;
        AllEarnings = empl.Earnings;
        AllSecondaryEarnings = empl.SecondaryEarnings;
        CurrentAmount = empl.CurrentAmount;
        EmployeeTypeId = empl.EmployeeTypeId;
        ContributionPoints = empl.PointsEarned;

        ZeroContributionReasonId = empl.ZeroContributionReasonId;
        Etva = empl.Etva;
        EarningsOnEtva = empl.EarningsOnEtva;
        SecondaryEtvaEarnings = empl.EarningsOnSecondaryEtva;

        Common(profitDetailTotals, memberTotals);
    }

    public MemberFinancials(BeneficiaryFinancials bene, ProfitDetailTotals profitDetailTotals, MemberTotals memberTotals)
    {
        IsEmployee = false;
        Badge = 0;
        Name = bene.Name;
        Ssn = bene.Ssn;
        Psn = bene.Psn;
        CurrentAmount = bene.CurrentAmount;
        AllSecondaryEarnings = bene.SecondaryEarnings;
        AllEarnings = bene.Earnings;

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
        AllEarnings = memberTotals.EarningsAmount;
        EarningPoints = memberTotals.EarnPoints;
        IncomingForfeitures = memberTotals.IncomingForfeitureAmount - profitDetailTotals.ForfeitsTotal;
    }

    public bool IsEmployee { get; set; }
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

    /// <summary>
    /// Includes earnings on all of the members money. (ie. non-vested, vested and ETVA)
    /// </summary>
    public decimal AllEarnings { get; set; } // PY-PROF-EARN

    /// <summary>
    /// 100% Vested Money
    /// </summary>
    public decimal Etva { get; set; } // PAYPROFIT.PY_PS_ETVA (a portion of the full current amount)

    /// <summary>
    /// Earnings on the ETVA portion of a members money
    /// </summary>
    public decimal EarningsOnEtva { get; set; } // PY-PROF-EARN  (a portion of Earnings)

    /// <summary>
    /// Secondary Earnings.   When present this will become a Year Iteration = 2 in PROFIT_DETAIL
    /// </summary>
    public decimal AllSecondaryEarnings { get; set; } // PY-PROF-EARN2

    /// <summary>
    /// Earnins on the Secondary ETVA portion of a member money.  This makes a Year Iteration = 2, with profit code = 8
    /// </summary>
    public decimal SecondaryEtvaEarnings { get; set; } // PY_PROF_ETVA2

    public decimal MaxOver { get; set; }
    public int MaxPoints { get; set; }
    public decimal Caf { get; set; }
    public byte? ZeroContributionReasonId { get; set; }

    public decimal EndingBalance =>
        CurrentAmount
        + Contributions
        + Xfer
        - Pxfer
        + AllEarnings
        + AllSecondaryEarnings
        + IncomingForfeitures
        + Military
        + Caf
        - Distributions;

    public bool IsAllZeros() => CurrentAmount == 0m &&
                                Distributions == 0m &&
                                Contributions == 0m &&
                                Xfer == 0m &&
                                Pxfer == 0m &&
                                Military == 0m &&
                                IncomingForfeitures == 0m &&
                                AllEarnings == 0m &&
                                AllSecondaryEarnings == 0m &&
                                EndingBalance == 0m;
}
