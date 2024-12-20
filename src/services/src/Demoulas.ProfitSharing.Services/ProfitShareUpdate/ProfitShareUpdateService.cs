using System.Diagnostics;
using System.Threading;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.ServiceDto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.ProfitShareUpdate;

/// <summary>
///     Does the Year And application of Earnings and Contributions to all employees and beneficiaries.
///     Modeled very closely after Pay444
/// </summary>
public class ProfitShareUpdateService : IProfitShareUpdateService
{
    private readonly ICalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dbContextFactory;
    private readonly ITotalService _totalService;

    public ProfitShareUpdateService(IProfitSharingDataContextFactory dbContextFactory, ITotalService totalService, ICalendarService calendarService)
    {
        _dbContextFactory = dbContextFactory;
        _totalService = totalService;
        _calendarService = calendarService;
    }

    public async Task<ProfitShareUpdateResponse> ProfitSharingUpdate(ProfitSharingUpdateRequest profitSharingUpdateRequest, CancellationToken cancellationToken)
    {
        (List<MemberFinancials> memberFinancials, _, bool employeeExceededMaxContribution) = await ProfitSharingUpdatePaginated(profitSharingUpdateRequest, cancellationToken);
        List<MemberFinancialsResponse> members = memberFinancials.Select(m => new MemberFinancialsResponse
        {
            Badge = m.Badge,
            Psn = m.Psn,
            Name = m.Name,
            CurrentAmount = m.CurrentAmount,
            Distributions = m.Distributions,
            Military = m.Military,
            Xfer = m.Xfer,
            Pxfer = m.Pxfer,
            EmployeeTypeId = m.EmployeeTypeId,
            Contributions = m.Contributions,
            IncomingForfeitures = m.IncomingForfeitures,
            Earnings = m.Earnings,
            SecondaryEarnings = m.SecondaryEarnings,
            EndingBalance = m.EndingBalance
        }).ToList();

        return new ProfitShareUpdateResponse
        {
            IsReRunRequired = employeeExceededMaxContribution,
            ReportName = "Profit Sharing Update",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<MemberFinancialsResponse> { Results = members }
        };
    }

    /// <summary>
    ///     Applies updates specified in request and returns members with updated Contributions/Earnings/IncomingForfeitures/SecondaryEarnings
    /// </summary>
    public async Task<ProfitShareUpdateOutcome> ProfitSharingUpdatePaginated(ProfitSharingUpdateRequest profitSharingUpdateRequest, CancellationToken cancellationToken)
    {
        // Values collected for an "Adjustment Report" that we do not yet generate
        AdjustmentReportData adjustmentReportData = new();

        List<MemberFinancials> members = new();
        bool employeeExceededMaxContribution = await ProcessEmployees(members, profitSharingUpdateRequest, adjustmentReportData, cancellationToken);
        await ProcessBeneficiaries(members, profitSharingUpdateRequest, cancellationToken);

        return new(members, adjustmentReportData, employeeExceededMaxContribution);
    }

    private async Task<bool> ProcessEmployees(List<MemberFinancials> members, ProfitSharingUpdateRequest profitSharingUpdateRequest,
        AdjustmentReportData adjustmentReportData, CancellationToken cancellationToken)
    {
        var employeeExceededMaxContribution = false;
        var fiscalDates = await _calendarService.GetYearStartAndEndAccountingDatesAsync(profitSharingUpdateRequest.ProfitYear, cancellationToken);
        List<EmployeeFinancials> employeeFinancialsList = await _dbContextFactory.UseReadOnlyContext(async ctx =>
        {
            var employees = await ctx.PayProfits
                .Include(pp => pp.Demographic)
                .Include(pp => pp.Demographic!.ContactInfo)
                .Where(pp => pp.ProfitYear == profitSharingUpdateRequest.ProfitYear)
                .Select(x => new
                {
                    EmployeeId = x.Demographic!.EmployeeId,
                    Ssn = x.Demographic.Ssn,
                    Name = x.Demographic.ContactInfo!.FullName,
                    EnrolledId = x.EnrollmentId,
                    YearsInPlan = x.YearsInPlan,
                    EmployeeTypeId = x.EmployeeTypeId,
                    PointsEarned = (int)(x.PointsEarned ?? 0),
                }).ToListAsync(cancellationToken);
            var ssns = employees.Select(e => e.Ssn);
            var totalVestingBalances = await ((TotalService)_totalService).TotalVestingBalance(ctx,
                    (short)(profitSharingUpdateRequest.ProfitYear - 1), fiscalDates.FiscalEndDate)
                    .Where(e=>ssns.Contains(e.Ssn)).ToListAsync(cancellationToken);

            return (
                from e in employees
                join t in totalVestingBalances on e.Ssn equals t.Ssn into t_join
                from tvb in t_join.DefaultIfEmpty()
                select new EmployeeFinancials
                {
                    EmployeeId = e.EmployeeId,
                    Ssn = e.Ssn,
                    Name = e.Name,
                    EnrolledId = e.EnrolledId,
                    YearsInPlan = e.YearsInPlan,
                    CurrentAmount = tvb == null ? 0 : tvb.CurrentBalance,
                    EmployeeTypeId = e.EmployeeTypeId,
                    PointsEarned =  e.PointsEarned,
                    EtvaAfterVestingRules = tvb == null ? 0 : tvb.Etva
                }
            ).ToList();
        });

        foreach (EmployeeFinancials empl in employeeFinancialsList)
        {
            // if employee is not participating 
            if (empl.EnrolledId != Enrollment.Constants.NotEnrolled || empl.YearsInPlan != 0)
            {
                var (memb, didEmployeeExceededMaxContribution) = await ProcessEmployee(empl, profitSharingUpdateRequest, adjustmentReportData, cancellationToken);
                members.Add(memb);
                employeeExceededMaxContribution |= didEmployeeExceededMaxContribution;
            }
        }

        return employeeExceededMaxContribution;
    }

    private async Task ProcessBeneficiaries(List<MemberFinancials> members, ProfitSharingUpdateRequest profitSharingUpdateRequest, CancellationToken cancellationToken)
    {
        List<BeneficiaryFinancials> benes = await _dbContextFactory.UseReadOnlyContext(ctx =>
            ctx.Beneficiaries.OrderBy(b => b.Contact!.ContactInfo.FullName)
                .ThenByDescending(b => b.EmployeeId * 10000 + b.PsnSuffix).Select(b =>
                    new BeneficiaryFinancials
                    {
                        Psn = Convert.ToInt64(b.Psn),
                        Ssn = b.Contact!.Ssn,
                        Name = b.Contact.ContactInfo.FullName,
                        CurrentAmount = b.Amount, // Should be computing this from the ProfitDetail via TotalService
                    }).ToListAsync(cancellationToken)
        );

        foreach (BeneficiaryFinancials bene in benes)
        {
            // is already handled as an employee?
            if (members.Any(m => m.Ssn == bene.Ssn))
            {
                continue;
            }

            MemberFinancials memb = await ProcessBeneficiary(bene, profitSharingUpdateRequest, cancellationToken);
            if (!memb.IsAllZeros())
            {
                members.Add(memb);
            }
        }
    }

    private async Task<(MemberFinancials, bool)> ProcessEmployee(EmployeeFinancials empl, ProfitSharingUpdateRequest profitSharingUpdateRequest,
        AdjustmentReportData adjustmentReportData, CancellationToken cancellationToken)
    {
        // Gets this year's profit sharing transactions, aka Distributions - hardships - Military - ClassActionFund
        ProfitDetailTotals profitDetailTotals =
            await ProfitDetailTotals.GetProfitDetailTotals(_dbContextFactory, profitSharingUpdateRequest.ProfitYear, empl.Ssn, cancellationToken);

        // MemberTotals holds newly computed values, not old values
        MemberTotals memberTotals = new();

        memberTotals.ContributionAmount =
            ComputeContribution(empl.PointsEarned, empl.EmployeeId, profitSharingUpdateRequest, adjustmentReportData);
        memberTotals.IncomingForfeitureAmount =
            ComputeForfeitures(empl.PointsEarned, empl.EmployeeId, profitSharingUpdateRequest, adjustmentReportData);

        // This "EarningsBalance" is actually the new Current Balance.  Consider changing the name
        // Note that CAF gets added here, but subtracted in the next line.   Odd.
        memberTotals.NewCurrentAmount = profitDetailTotals.AllocationsTotal + profitDetailTotals.ClassActionFundTotal +
                                        (empl.CurrentAmount - profitDetailTotals.ForfeitsTotal -
                                         profitDetailTotals.PaidAllocationsTotal) -
                                        profitDetailTotals.DistributionsTotal;
        memberTotals.NewCurrentAmount -= profitDetailTotals.ClassActionFundTotal;

        if (memberTotals.NewCurrentAmount > 0)
        {
            memberTotals.PointsDollars = Math.Round(memberTotals.NewCurrentAmount, 2, MidpointRounding.AwayFromZero);
            memberTotals.EarnPoints = (int)Math.Round(memberTotals.PointsDollars / 100, MidpointRounding.AwayFromZero);
        }

        ComputeEarningsEmployee(empl, memberTotals, profitSharingUpdateRequest, adjustmentReportData, profitDetailTotals.ClassActionFundTotal);

        MemberFinancials memberFinancials = new(empl, profitDetailTotals, memberTotals);

        //   --- Max Contribution Concerns --- 
        decimal memberTotalContribution = memberTotals.ContributionAmount + profitDetailTotals.MilitaryTotal +
                                          memberTotals.IncomingForfeitureAmount;

        bool employeeExceededMaxContribution = false;
        if (memberTotalContribution > profitSharingUpdateRequest.MaxAllowedContributions)
        {
            decimal overContribution = memberTotalContribution - profitSharingUpdateRequest.MaxAllowedContributions;

            if (overContribution < memberTotals.IncomingForfeitureAmount)
            {
                memberFinancials.IncomingForfeitures -= overContribution;
            }
            else
            {
                memberFinancials.IncomingForfeitures = 0;
            }

            memberFinancials.MaxOver = overContribution;
            memberFinancials.MaxPoints = memberFinancials.ContributionPoints;
            employeeExceededMaxContribution = true;
        }
        // --- End Max Contribution

        empl.Contributions = memberTotals.ContributionAmount;
        empl.IncomeForfeiture = memberTotals.IncomingForfeitureAmount;
        return (memberFinancials, employeeExceededMaxContribution);
    }

    private async Task<MemberFinancials> ProcessBeneficiary(BeneficiaryFinancials bene, ProfitSharingUpdateRequest profitSharingUpdateRequest, CancellationToken cancellationToken)
    {
        var profitDetailTotals =
            await ProfitDetailTotals.GetProfitDetailTotals(_dbContextFactory, profitSharingUpdateRequest.ProfitYear, bene.Ssn, cancellationToken);

        MemberTotals memberTotals = new();
        // Yea, this adding and removing ClassActionFundTotal is strange
        memberTotals.NewCurrentAmount = profitDetailTotals.AllocationsTotal + profitDetailTotals.ClassActionFundTotal +
                                        (bene.CurrentAmount - profitDetailTotals.ForfeitsTotal -
                                         profitDetailTotals.PaidAllocationsTotal) -
                                        profitDetailTotals.DistributionsTotal;
        memberTotals.NewCurrentAmount -= profitDetailTotals.ClassActionFundTotal;

        if (memberTotals.NewCurrentAmount > 0)
        {
            memberTotals.PointsDollars = Math.Round(memberTotals.NewCurrentAmount, 2, MidpointRounding.AwayFromZero);
            memberTotals.EarnPoints = (int)Math.Round(memberTotals.PointsDollars / 100, MidpointRounding.AwayFromZero);
        }

        ComputeEarningsBeneficiary(memberTotals, bene, profitSharingUpdateRequest);

        return new MemberFinancials(bene, profitDetailTotals, memberTotals);
    }


    private static decimal ComputeContribution(long pointsEarned, long badge, ProfitSharingUpdateRequest profitSharingUpdateRequest,
        AdjustmentReportData adjustmentReportData)
    {
        decimal contributionAmount = Math.Round(profitSharingUpdateRequest.ContributionPercent * pointsEarned, 2,
            MidpointRounding.AwayFromZero);

        if (profitSharingUpdateRequest.BadgeToAdjust > 0 && profitSharingUpdateRequest.BadgeToAdjust == badge)
        {
            adjustmentReportData.ContributionAmountUnadjusted = contributionAmount;
            contributionAmount += profitSharingUpdateRequest.AdjustContributionAmount;
            adjustmentReportData.ContributionAmountAdjusted = contributionAmount;
        }

        return contributionAmount;
    }


    private static decimal ComputeForfeitures(long pointsEarned, long badge, ProfitSharingUpdateRequest profitSharingUpdateRequest,
        AdjustmentReportData adjustmentReportData)
    {
        decimal incomingForfeitureAmount = Math.Round(profitSharingUpdateRequest.IncomingForfeitPercent * pointsEarned, 2, MidpointRounding.AwayFromZero);
        if (profitSharingUpdateRequest.BadgeToAdjust > 0 && profitSharingUpdateRequest.BadgeToAdjust == badge)
        {
            adjustmentReportData.IncomingForfeitureAmountUnadjusted = incomingForfeitureAmount;
            incomingForfeitureAmount += profitSharingUpdateRequest.AdjustIncomingForfeitAmount;
            adjustmentReportData.IncomingForfeitureAmountAdjusted = incomingForfeitureAmount;
        }

        return incomingForfeitureAmount;
    }

    // The fact that this method takes either a bene or an empl and has all this conditional logic is not great.
    private static void ComputeEarningsEmployee(EmployeeFinancials empl, MemberTotals memberTotals, ProfitSharingUpdateRequest profitSharingUpdateRequest,
        AdjustmentReportData? adjustmentsApplied, decimal classActionFundTotal)
    {
        if (memberTotals.EarnPoints <= 0)
        {
            memberTotals.EarnPoints = 0;
            empl.Earnings = 0;
            empl.SecondaryEarnings = 0;
        }

        memberTotals.EarningsAmount = Math.Round(profitSharingUpdateRequest.EarningsPercent * memberTotals.EarnPoints, 2,
            MidpointRounding.AwayFromZero);
        if (profitSharingUpdateRequest.BadgeToAdjust > 0 && profitSharingUpdateRequest.BadgeToAdjust == (empl?.EmployeeId ?? 0))
        {
            adjustmentsApplied!.EarningsAmountUnadjusted = memberTotals.EarningsAmount;
            memberTotals.EarningsAmount += profitSharingUpdateRequest.AdjustEarningsAmount;
            adjustmentsApplied.EarningsAmountAdjusted = memberTotals.EarningsAmount;
        }

        memberTotals.SecondaryEarningsAmount =
            Math.Round(profitSharingUpdateRequest.SecondaryEarningsPercent * memberTotals.EarnPoints, 2,
                MidpointRounding.AwayFromZero);
        if (profitSharingUpdateRequest.BadgeToAdjust2 > 0 && profitSharingUpdateRequest.BadgeToAdjust2 == (empl?.EmployeeId ?? 0))
        {
            adjustmentsApplied!.SecondaryEarningsAmountUnadjusted = memberTotals.SecondaryEarningsAmount;
            memberTotals.SecondaryEarningsAmount += profitSharingUpdateRequest.AdjustEarningsSecondaryAmount;
            adjustmentsApplied.SecondaryEarningsAmountAdjusted = memberTotals.SecondaryEarningsAmount;
        }

        decimal etvaAfterVestingRulesAdjustedByCaf = AdjustEmployeeEarningsForClassActionFund(empl!, memberTotals, classActionFundTotal);

        if (profitSharingUpdateRequest.SecondaryEarningsPercent == 0m) // Secondary Earnings
        {
            return;
        }

        decimal etvaScaled = etvaAfterVestingRulesAdjustedByCaf / memberTotals.PointsDollars;
        decimal etvaSecondaryScaledAmount = Math.Round(memberTotals.SecondaryEarningsAmount * etvaScaled, 2,
            MidpointRounding.AwayFromZero);
        memberTotals.SecondaryEarningsAmount -= etvaSecondaryScaledAmount;
        empl!.SecondaryEarnings = memberTotals.SecondaryEarningsAmount;
        empl.SecondaryEtvaEarnings = etvaSecondaryScaledAmount;
    }

    private static void ComputeEarningsBeneficiary(MemberTotals memberTotals, BeneficiaryFinancials bene, ProfitSharingUpdateRequest profitSharingUpdateRequest)
    {
        memberTotals.EarningsAmount = Math.Round(profitSharingUpdateRequest.EarningsPercent * memberTotals.EarnPoints, 2,
            MidpointRounding.AwayFromZero);

        bene!.Earnings = memberTotals.EarningsAmount;

        memberTotals.SecondaryEarningsAmount =
            Math.Round(profitSharingUpdateRequest.SecondaryEarningsPercent * memberTotals.EarnPoints, 2,
                MidpointRounding.AwayFromZero);

        bene.SecondaryEarnings = memberTotals.SecondaryEarningsAmount;
    }

    // This comment from cobol helps explain the following method.
    //* -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //* ETVA EARNINGS ARE CALCULATED AND WRITTEN TO PY-PROF-ETVA (EtvaAfterVestingRules)
    //* -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //* need to subtract CAF out of PY-PS-ETVA (EtvaAfterVestingRules) for people not fully vested
    //* because  we can't give earnings for 2021 on class action funds -
    //* they were added in 2021.CAF was added to PY-PS-ETVA (EtvaAfterVestingRules) for
    //* PY-PS-YEARS < 6.
    private static decimal AdjustEmployeeEarningsForClassActionFund(EmployeeFinancials empl, MemberTotals memberTotals, decimal classActionFundTotal)
    {
        decimal etvaAfterVestingRulesAdjustedByCaf = 0;
        if (empl.EtvaAfterVestingRules > 0)
        {
            if (empl.YearsInPlan < 6) // This 6 is only used here, so it is intentionally not pulled out.  
            {
                etvaAfterVestingRulesAdjustedByCaf = empl.EtvaAfterVestingRules - classActionFundTotal;
            }
            else
            {
                empl.EtvaAfterVestingRules = etvaAfterVestingRulesAdjustedByCaf;
            }
        }

        if (etvaAfterVestingRulesAdjustedByCaf <= 0)
        {
            empl.Earnings = memberTotals.EarningsAmount;
            empl.SecondaryEarnings = memberTotals.SecondaryEarningsAmount;
            empl.EarningsOnEtva = 0m;
            empl.SecondaryEtvaEarnings = 0m;
            return 0;
        }

        if (memberTotals.PointsDollars <= 0)
        {
            return etvaAfterVestingRulesAdjustedByCaf;
        }

        // Computes the ETVA amount
        decimal etvaScaled = etvaAfterVestingRulesAdjustedByCaf / memberTotals.PointsDollars;
        decimal etvaScaledAmount =
            Math.Round(memberTotals.EarningsAmount * etvaScaled, 2, MidpointRounding.AwayFromZero);

        // subtracts that amount from the members total earnings
        memberTotals.EarningsAmount -= etvaScaledAmount;

        // Sets Earn and ETVA amounts
        empl!.Earnings = memberTotals.EarningsAmount;
        empl.EarningsOnEtva = etvaScaledAmount;

        return etvaAfterVestingRulesAdjustedByCaf;
    }
}
