using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.Services.Reports.ProfitShareUpdate;


/// <summary>
///     Does the Year And application of Earnings and Contributions to all employees and beneficiaries.
///     Modeled very closely after Pay444
/// </summary>
public class ProfitShareUpdateService
{
    // We are currently hooked up to the PROFITSHARE database for employees because we do not yet have a way to correctly calculate ETVA
    private EmployeeDataHelper _employeeDataHelper;

    private readonly IProfitSharingDataContextFactory dbContextFactory;
    private readonly ILogger<ProfitShareUpdateService> _logger;

    // Need to ensure this is surfaced correctly
    private bool _rerunNeeded;

    private readonly OracleConnection connection;

    public ProfitShareUpdateService(OracleConnection connection, IProfitSharingDataContextFactory dbContextFactory, ILoggerFactory loggerFactory)
    {
        this.dbContextFactory = dbContextFactory;
        this.connection = connection;
        this._logger = loggerFactory.CreateLogger<ProfitShareUpdateService>();
    }

    public long ProfitYear { get; set; }

    public DateTime TodaysDateTime { get; set; } = DateTime.Now;

    public (List<MemberFinancials>, AdjustmentsApplied, bool) ApplyAdjustments(short profitYear, decimal contributionPercent,
        decimal incomingForfeitPercent, decimal earningsPercent, decimal secondaryEarningsPercent,
        long badgeToAdjust, decimal adjustContributionAmount, decimal adjustIncomingForfeitAmount,
        decimal adjustEarningsAmount,
        long badgeToAdjust2, decimal adjustEarningsSecondary, long maxAllowedContributions)
    {
        // Temporary until PY_PROF_ETVA lookup is fixed.
        _employeeDataHelper = new EmployeeDataHelper(connection, dbContextFactory, profitYear);

        // Should AdjustmentAmounts be a request DTO?
        AdjustmentAmounts adjustmentAmounts = new AdjustmentAmounts(
            contributionPercent,
            incomingForfeitPercent,
            earningsPercent,
            secondaryEarningsPercent,
            BadgeToAdjust: badgeToAdjust,
            AdjustContributionAmount: adjustContributionAmount,
            AdjustIncomingForfeitAmount: adjustIncomingForfeitAmount,
            AdjustEarningsAmount: adjustEarningsAmount,
            BadgeToAdjust2: badgeToAdjust2,
            AdjustEarningsSecondaryAmount: adjustEarningsSecondary,
            MaxAllowedContributions: maxAllowedContributions
        );

        // Values collected for an "Adjustment Report" that we do not yet generate
        AdjustmentsApplied adjustmentsApplied = new();

        this.ProfitYear = profitYear;


        List<MemberFinancials> members = new();
        ProcessEmployees(members, adjustmentAmounts, adjustmentsApplied);
        ProcessBeneficiaries(members, adjustmentAmounts);

        if (_rerunNeeded)
        {
            // BOBH This needs to get back to the user - so they can make adjustments for MAX_CONTRIBUTIONS?
            _logger.LogError("Rerun of PAY444 is required.  See output for issues.");
        }

        return (members, adjustmentsApplied, _rerunNeeded);
    }

    public void ProcessEmployees(List<MemberFinancials> members, AdjustmentAmounts adjustmentAmounts,
        AdjustmentsApplied adjustmentsApplied)
    {
        foreach (EmployeeFinancials empl in _employeeDataHelper.rows)
        {
            MemberFinancials? memb = ProcessEmployee(empl, adjustmentAmounts, adjustmentsApplied);
            if (memb != null)
            {
                members.Add(memb);
            }
        }
    }

    private void ProcessBeneficiaries(List<MemberFinancials> members, AdjustmentAmounts adjustmentAmounts)
    {
        List<BeneficiaryFinancials> benes = dbContextFactory.UseReadOnlyContext(ctx =>
            ctx.Beneficiaries.OrderBy(b => b.Contact.ContactInfo.FullName)
                .ThenByDescending(b => b.EmployeeId * 10000 + b.PsnSuffix).Select(b =>
                    new BeneficiaryFinancials
                    {
                        Psn = Convert.ToInt64(b.Psn),
                        Ssn = b.Contact.Ssn,
                        Name = b.Contact.ContactInfo.FullName,
                        CurrentAmount = b.Amount,
                        Earnings = b.Earnings,
                        SecondaryEarnings = b.SecondaryEarnings
                    }).ToListAsync()
        ).GetAwaiter().GetResult();

        foreach (BeneficiaryFinancials bene in benes)
        {
            // is already handled as an employee?
            if (_employeeDataHelper.HasRecordBySsn(bene.Ssn))
            {
                continue;
            }

            MemberFinancials? memb = ProcessBeneficiary(bene, adjustmentAmounts);
            if (memb != null)
            {
                members.Add(memb);
            }
        }
    }

    public MemberFinancials? ProcessEmployee(EmployeeFinancials empl, AdjustmentAmounts adjustmentAmounts,
        AdjustmentsApplied adjustmentsApplied)
    {
        //* If an employee has an ETVA amount and no years on the plan, employee is a
        //* beneficiary and should get earnings on the etva amt (8 record)
        if (empl.EmployeeTypeId == 0) // 0 = not new, 1 == new in plan
        {
            if (empl.EtvaAfterVestingRules > 0 && empl.CurrentAmount == 0)
            {
                empl.EmployeeTypeId = 2; // empl is bene and gets earning on ETVA
            }
        }

        if (empl.EnrolledId <= Enrollment.Constants.NotEnrolled && empl.EmployeeTypeId <= 0 &&
            empl.CurrentAmount <= 0 && empl.YearsInPlan <= 0)
        {
            return null;
        }

        DetailTotals detailTotals = GetDetailTotals(empl.Ssn);

        MemberTotals memberTotals = new();
        memberTotals.ContributionAmount =
            ComputeContribution(empl.PointsEarned, empl.EmployeeId, adjustmentAmounts, adjustmentsApplied);
        memberTotals.IncomingForfeitureAmount =
            ComputeForfeitures(empl.PointsEarned, empl.EmployeeId, adjustmentAmounts, adjustmentsApplied);


        memberTotals.EarningsBalance = detailTotals.AllocationsTotal + detailTotals.ClassActionFundTotal +
                                       (empl.CurrentAmount - detailTotals.ForfeitsTotal -
                                        detailTotals.PaidAllocationsTotal) -
                                       detailTotals.DistributionsTotal;

        memberTotals.EarningsBalance -= detailTotals.ClassActionFundTotal;

        if (memberTotals.EarningsBalance <= 0)
        {
            memberTotals.EarnPoints = 0;
            memberTotals.PointsDollars = 0;
        }
        else
        {
            memberTotals.PointsDollars = Math.Round(memberTotals.EarningsBalance, 2, MidpointRounding.AwayFromZero);
            memberTotals.EarnPoints = (long)Math.Round(memberTotals.PointsDollars / 100, MidpointRounding.AwayFromZero);
        }

        ComputeEarnings(memberTotals, null, empl, adjustmentAmounts, adjustmentsApplied, detailTotals.ClassActionFundTotal);

        MemberFinancials memb = new();
        memb.EmployeeId = empl.EmployeeId;
        memb.Psn = empl.EmployeeId;
        memb.Name = empl.Name;
        memb.Ssn = empl.Ssn;
        memb.Xfer = detailTotals.AllocationsTotal;
        memb.Pxfer = detailTotals.PaidAllocationsTotal;
        memb.CurrentAmount = empl.CurrentAmount;
        memb.Distributions = detailTotals.DistributionsTotal;
        memb.Military = detailTotals.MilitaryTotal;
        memb.Caf = detailTotals.ClassActionFundTotal;
        memb.EmployeeTypeId = empl.EmployeeTypeId;
        memb.ContributionPoints = empl.PointsEarned;
        memb.EarningPoints = memberTotals.EarnPoints;
        memb.Contributions = memberTotals.ContributionAmount;
        memb.IncomingForfeitures = memberTotals.IncomingForfeitureAmount;
        memb.IncomingForfeitures -= detailTotals.ForfeitsTotal;
        memb.Earnings = empl.Earnings;
        memb.Earnings += empl.EarningsOnEtva;
        memb.SecondaryEarnings = empl.SecondaryEarnings;
        memb.SecondaryEarnings += empl.SecondaryEtvaEarnings;

        decimal memberTotalContribution = memberTotals.ContributionAmount + detailTotals.MilitaryTotal +
                                          memberTotals.IncomingForfeitureAmount;
        if (memberTotalContribution > adjustmentAmounts.MaxAllowedContributions)
        {
            decimal overContribution = memberTotalContribution - adjustmentAmounts.MaxAllowedContributions;

            if (overContribution < memberTotals.IncomingForfeitureAmount)
            {
                memb.IncomingForfeitures -= overContribution;
            }
            else
            {
                _logger.LogError($"FORFEITURES NOT ENOUGH FOR AMOUNT OVER MAX FOR EMPLOYEE BADGE #{empl.EmployeeId}");
                memb.IncomingForfeitures = 0;
            }

            memb.MaxOver = overContribution;
            memb.MaxPoints = memb.ContributionPoints;
            _rerunNeeded = true;
        }

        empl.Contributions = memberTotals.ContributionAmount;
        empl.IncomeForfeiture = memberTotals.IncomingForfeitureAmount;
        return memb;
    }


    public MemberFinancials ProcessBeneficiary(BeneficiaryFinancials bene, AdjustmentAmounts adjustmentAmounts)
    {
        DetailTotals detailTotals = GetDetailTotals(bene.Ssn);

        // Yea, this adding and removing ClassActionFundTotal is strange
        MemberTotals memberTotals = new();
        memberTotals.EarningsBalance = detailTotals.AllocationsTotal + detailTotals.ClassActionFundTotal +
                                       (bene.CurrentAmount - detailTotals.ForfeitsTotal -
                                        detailTotals.PaidAllocationsTotal) -
                                       detailTotals.DistributionsTotal;
        memberTotals.EarningsBalance -= detailTotals.ClassActionFundTotal;

        if (memberTotals.EarningsBalance > 0)
        {
            memberTotals.PointsDollars = Math.Round(memberTotals.EarningsBalance, 2, MidpointRounding.AwayFromZero);
            memberTotals.EarnPoints = (long)Math.Round(memberTotals.PointsDollars / 100, MidpointRounding.AwayFromZero);
        }

        ComputeEarnings(memberTotals, bene, null, adjustmentAmounts, null, detailTotals.ClassActionFundTotal);

        MemberFinancials memb = new();
        memb.Name = bene.Name;
        memb.Ssn = bene.Ssn;
        memb.Psn = bene.Psn;
        memb.Distributions = detailTotals.DistributionsTotal;
        memb.Caf = detailTotals.ClassActionFundTotal > 0 ? detailTotals.ClassActionFundTotal : 0;
        memb.Xfer = detailTotals.AllocationsTotal;
        memb.Pxfer = detailTotals.PaidAllocationsTotal;
        memb.CurrentAmount = bene.CurrentAmount;
        memb.EarningPoints = memberTotals.EarnPoints;
        memb.IncomingForfeitures -= detailTotals.ForfeitsTotal;
        memb.Earnings = bene.Earnings;
        memb.SecondaryEarnings = bene.SecondaryEarnings;
        return memb;
    }


    private static decimal ComputeContribution(long ws_payprofit, long badge, AdjustmentAmounts adjustmentAmounts,
        AdjustmentsApplied adjustmentsApplied)
    {
        decimal contributionAmount = Math.Round(adjustmentAmounts.ContributionPercent * ws_payprofit, 2,
            MidpointRounding.AwayFromZero);

        if (adjustmentAmounts.BadgeToAdjust > 0 && adjustmentAmounts.BadgeToAdjust == badge)
        {
            adjustmentsApplied.ContributionAmountUnadjusted = contributionAmount;
            contributionAmount += adjustmentAmounts.AdjustContributionAmount;
            adjustmentsApplied.ContributionAmountAdjusted = contributionAmount;
        }

        return contributionAmount;
    }


    private static decimal ComputeForfeitures(long ws_payprofit, long badge, AdjustmentAmounts adjustmentAmounts,
        AdjustmentsApplied adjustmentsApplied)
    {
        decimal incomingForfeitureAmount = Math.Round(adjustmentAmounts.IncomingForfeitPercent * ws_payprofit, 2,
            MidpointRounding.AwayFromZero);
        if (adjustmentAmounts.BadgeToAdjust > 0 && adjustmentAmounts.BadgeToAdjust == badge)
        {
            adjustmentsApplied.IncomingForfeitureAmountUnadjusted = incomingForfeitureAmount;
            incomingForfeitureAmount += adjustmentAmounts.AdjustIncomingForfeitAmount;
            adjustmentsApplied.IncomingForfeitureAmountAdjusted = incomingForfeitureAmount;
        }

        return incomingForfeitureAmount;
    }

    public void ComputeEarnings(MemberTotals memberTotals, BeneficiaryFinancials? bene, EmployeeFinancials? empl,
        AdjustmentAmounts adjustmentAmounts, AdjustmentsApplied? adjustmentsApplied, decimal ClassActionFundTotal)
    {
        if (memberTotals.EarnPoints <= 0 && empl != null)
        {
            memberTotals.EarnPoints = 0;
            memberTotals.EarningsAmount = 0;
            memberTotals.SecondaryEarningsAmount = 0;
            empl.Earnings = 0;
            empl.SecondaryEarnings = 0;
        }

        memberTotals.EarningsAmount = Math.Round(adjustmentAmounts.EarningsPercent * memberTotals.EarnPoints, 2,
            MidpointRounding.AwayFromZero);
        if (adjustmentAmounts.BadgeToAdjust > 0 && adjustmentAmounts.BadgeToAdjust == (empl?.EmployeeId ?? 0))
        {
            adjustmentsApplied.EarningsAmountUnadjusted = memberTotals.EarningsAmount;
            memberTotals.EarningsAmount += adjustmentAmounts.AdjustEarningsAmount;
            adjustmentsApplied.EarningsAmountAdjusted = memberTotals.EarningsAmount;
        }

        memberTotals.SecondaryEarningsAmount =
            Math.Round(adjustmentAmounts.SecondaryEarningsPercent * memberTotals.EarnPoints, 2,
                MidpointRounding.AwayFromZero);
        if (adjustmentAmounts.BadgeToAdjust2 > 0 && adjustmentAmounts.BadgeToAdjust2 == (empl?.EmployeeId ?? 0))
        {
            adjustmentsApplied.SecondaryEarningsAmountUnadjusted = memberTotals.SecondaryEarningsAmount;
            memberTotals.SecondaryEarningsAmount += adjustmentAmounts.AdjustEarningsSecondaryAmount;
            adjustmentsApplied.SecondaryEarningsAmountAdjusted = memberTotals.SecondaryEarningsAmount;
        }

        //* -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        //* ETVA EARNINGS ARE CALCULATED AND WRITTEN TO PY-PROF-ETVA
        //* -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        //* need to subtract CAF out of PY-PS-ETVA for people not fully vested
        //* because  we can't give earnings for 2021 on class action funds -
        //* they were added in 2021.CAF was added to PY - PS - ETVA for
        //* PY - PS - YEARS < 6.

        decimal EtvaAfterVestingRulesAdjustedByCAF = 0;
        if (empl != null && empl.EtvaAfterVestingRules > 0)
        {
            if (empl.YearsInPlan < 6)
            {
                EtvaAfterVestingRulesAdjustedByCAF = empl.EtvaAfterVestingRules - ClassActionFundTotal;
            }
            else
            {
                empl.EtvaAfterVestingRules = EtvaAfterVestingRulesAdjustedByCAF;
            }
        }

        if (EtvaAfterVestingRulesAdjustedByCAF <= 0 && empl != null)
        {
            empl.Earnings = memberTotals.EarningsAmount;
            empl.SecondaryEarnings = memberTotals.SecondaryEarningsAmount;
            empl.EarningsOnEtva = 0m;
            empl.SecondaryEtvaEarnings = 0m;
            return;
        }


        if (empl != null && memberTotals.PointsDollars > 0)
        {
            // Computes the ETVA amount
            decimal EtvaScaled = EtvaAfterVestingRulesAdjustedByCAF / memberTotals.PointsDollars;
            decimal EtvaScaledAmount =
                Math.Round(memberTotals.EarningsAmount * EtvaScaled, 2, MidpointRounding.AwayFromZero);

            // subtracts that amount from the members total earnings
            memberTotals.EarningsAmount = memberTotals.EarningsAmount - EtvaScaledAmount;

            // Sets Earn and ETVA amounts
            empl!.Earnings = memberTotals.EarningsAmount;
            empl.EarningsOnEtva = EtvaScaledAmount;
        }

        if (bene != null)
        {
            bene.Earnings = 0m;
            bene.Earnings = memberTotals.EarningsAmount;
        }

        if (adjustmentAmounts.SecondaryEarningsPercent != 0m) // Secondary Earnings
        {
            decimal EtvaScaled = EtvaAfterVestingRulesAdjustedByCAF / memberTotals.PointsDollars;
            decimal EtvaSecondaryScaledAmount = Math.Round(memberTotals.SecondaryEarningsAmount * EtvaScaled, 2,
                MidpointRounding.AwayFromZero);
            memberTotals.SecondaryEarningsAmount -= EtvaSecondaryScaledAmount;
            if (empl != null)
            {
                empl.SecondaryEarnings = memberTotals.SecondaryEarningsAmount;
                empl.SecondaryEtvaEarnings = EtvaSecondaryScaledAmount;
            }

            if (bene != null)
            {
                bene.SecondaryEarnings = EtvaSecondaryScaledAmount;
            }
        }
    }

    public DetailTotals GetDetailTotals(int ssn)
    {
        decimal distributionsTotal = 0;
        decimal forfeitsTotal = 0;
        decimal allocationsTotal = 0;
        decimal paidAllocationsTotal = 0;
        decimal militaryTotal = 0;
        decimal classActionFundTotal = 0;

        List<ProfitDetail> pds = dbContextFactory.UseReadOnlyContext(ctx =>
            ctx.ProfitDetails.Where(pd => pd.Ssn == ssn && pd.ProfitYear == ProfitYear)
                .OrderBy(pd => pd.ProfitYear).ThenBy(pd => pd.ProfitYearIteration).ThenBy(pd => pd.MonthToDate)
                .ThenBy(pd => pd.FederalTaxes)
                .ToListAsync()
        ).GetAwaiter().GetResult();

        foreach (ProfitDetail pd in pds)
        {
            (byte profitCode, string? remark, decimal forfeiture, decimal contribution, decimal earnings,
                byte profitYearIteration) = (pd.ProfitCodeId,
                pd.Remark, pd.Forfeiture, pd.Contribution, pd.Earnings, pd.ProfitYearIteration);

            if (profitCode == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal /*1*/ ||
                profitCode == ProfitCode.Constants.OutgoingDirectPayments /*3*/)
            {
                distributionsTotal += forfeiture;
            }

            if (profitCode == ProfitCode.Constants.Outgoing100PercentVestedPayment /*9*/)
            {
                if (remark![..6] == "XFER >" ||
                    remark[..6] == "QDRO >" ||
                    remark[..5] == "XFER>" ||
                    remark[..5] == "QDRO>")
                {
                    paidAllocationsTotal += forfeiture;
                }
                else
                {
                    distributionsTotal += forfeiture;
                }
            }

            if (profitCode == ProfitCode.Constants.OutgoingForfeitures /*2*/)
            {
                forfeitsTotal += forfeiture;
            }

            if (profitCode == ProfitCode.Constants.OutgoingXferBeneficiary /*5*/)
            {
                paidAllocationsTotal += forfeiture;
            }

            if (profitCode == ProfitCode.Constants.IncomingQdroBeneficiary /*6*/)
            {
                allocationsTotal += contribution;
            }

            if (profitYearIteration == 1 /*Military*/)
            {
                militaryTotal += contribution;
            }

            if (profitYearIteration == 2 /*Class Action Fund*/)
            {
                classActionFundTotal += earnings;
            }
        }

        return new DetailTotals(
            distributionsTotal,
            forfeitsTotal,
            allocationsTotal,
            paidAllocationsTotal,
            militaryTotal,
            classActionFundTotal);
    }

}
