using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.ProfitShareUpdate;

/// <summary>
///     Does the Year And application of Earnings and Contributions to all employees and beneficiaries.
///     Modeled very closely after Pay444
///
///     This class follows the name of the step in the Ready YE flow.    It could instead be named "View effect of YE update on members"
/// </summary>
public class ProfitShareUpdateService : IInternalProfitShareUpdateService
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

    public async Task<ProfitShareUpdateResponse> ProfitShareUpdate(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        (List<MemberFinancials> memberFinancials, _, bool employeeExceededMaxContribution) = await ProfitSharingUpdatePaginated(profitShareUpdateRequest, cancellationToken);
        List<ProfitShareUpdateMemberResponse> members = memberFinancials.Select(m => new ProfitShareUpdateMemberResponse
        {
            IsEmployee = m.IsEmployee,
            Badge = m.Badge,
            Psn = m.Psn,
            Name = m.Name,
            BeginningAmount = m.CurrentAmount,
            Distributions = m.Distributions,
            Military = m.Military,
            Xfer = m.Xfer,
            Pxfer = m.Pxfer,
            EmployeeTypeId = m.EmployeeTypeId,
            Contributions = m.Contributions,
            IncomingForfeitures = m.IncomingForfeitures,
            AllEarnings = m.AllEarnings,
            Etva = m.Etva,
            AllSecondaryEarnings = m.AllSecondaryEarnings,
            EtvaEarnings = m.EarningsOnEtva,
            SecondaryEtvaEarnings = m.SecondaryEtvaEarnings,
            EndingBalance = m.EndingBalance,
            ZeroContributionReasonId = m.ZeroContributionReasonId
        }).ToList();

        return new ProfitShareUpdateResponse
        {
            HasExceededMaximumContributions = employeeExceededMaxContribution,
            ReportName = "Profit Sharing Update",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<ProfitShareUpdateMemberResponse> { Results = members }
        };
    }

    public async Task<ProfitShareUpdateResult> ProfitShareUpdateInternal(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        (List<MemberFinancials> memberFinancials, _, bool employeeExceededMaxContribution) = await ProfitSharingUpdatePaginated(profitShareUpdateRequest, cancellationToken);
        List<ProfitShareUpdateMember> members = memberFinancials.Select(m => new ProfitShareUpdateMember
        {
            IsEmployee = m.IsEmployee,
            Ssn = m.Ssn,
            Badge = m.Badge,
            Psn = m.Psn,
            Name = m.Name,
            BeginningAmount = m.CurrentAmount,
            Distributions = m.Distributions,
            Military = m.Military,
            Xfer = m.Xfer,
            Pxfer = m.Pxfer,
            EmployeeTypeId = m.EmployeeTypeId,
            Contributions = m.Contributions,
            IncomingForfeitures = m.IncomingForfeitures,
            AllEarnings = m.AllEarnings,
            Etva = m.Etva,
            AllSecondaryEarnings = m.AllSecondaryEarnings,
            EtvaEarnings = m.EarningsOnEtva,
            SecondaryEtvaEarnings = m.SecondaryEtvaEarnings,
            EndingBalance = m.EndingBalance,
            ZeroContributionReasonId = m.ZeroContributionReasonId
        }).ToList();

        return new ProfitShareUpdateResult()
        {
            HasExceededMaximumContributions = employeeExceededMaxContribution,
            Members = members
        };
    }

    /// <summary>
    ///     Applies updates specified in request and returns members with updated Contributions/Earnings/IncomingForfeitures/SecondaryEarnings
    /// </summary>
    public async Task<ProfitShareUpdateOutcome> ProfitSharingUpdatePaginated(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        // Values collected for an "Adjustment Report" that we do not yet generate
        AdjustmentReportData adjustmentReportData = new();

        List<MemberFinancials> members = new();
        bool employeeExceededMaxContribution = await ProcessEmployees(members, profitShareUpdateRequest, adjustmentReportData, cancellationToken);
        await ProcessBeneficiaries(members, profitShareUpdateRequest, cancellationToken);

        return new(members, adjustmentReportData, employeeExceededMaxContribution);
    }

    private async Task<bool> ProcessEmployees(List<MemberFinancials> members, ProfitShareUpdateRequest profitShareUpdateRequest,
        AdjustmentReportData adjustmentReportData, CancellationToken cancellationToken)
    {
        var employeeExceededMaxContribution = false;
        short currentYear = profitShareUpdateRequest.ProfitYear;
        short priorYear = (short)(profitShareUpdateRequest.ProfitYear - 1);
        
        // We want everything up to the beginning of this currentYear year, so we use lastYear in this lookup.
        var fiscalDates = await _calendarService.GetYearStartAndEndAccountingDatesAsync(priorYear, cancellationToken);
        List<EmployeeFinancials> employeeFinancialsList = await _dbContextFactory.UseReadOnlyContext(async ctx =>
        {
            var employees = await ctx.PayProfits
                .Include(pp => pp.Demographic) //Question - Should this be referring to frozen demographics
                .Include(pp => pp.Demographic!.ContactInfo)
                .Where(pp => pp.ProfitYear == currentYear)
                .Select(x => new
                {
                    x.Demographic!.BadgeNumber,
                    x.Demographic.Ssn,
                    Name = x.Demographic.ContactInfo!.FullName,
                    EnrolledId = x.EnrollmentId,
                    x.YearsInPlan,
                    x.EmployeeTypeId,
                    PointsEarned = (int)(x.PointsEarned ?? 0),
                    x.ZeroContributionReasonId,
                }).ToListAsync(cancellationToken);
            var ssns = employees.Select(e => e.Ssn);
            var totalVestingBalances = await ((TotalService)_totalService)
                .TotalVestingBalance(ctx, currentYear, priorYear, fiscalDates.FiscalEndDate)
                .Where(e => ssns.Contains(e.Ssn)).ToListAsync(cancellationToken);
            return
                employees
                    .GroupJoin(
                        totalVestingBalances,
                        e => e.Ssn,
                        t => t.Ssn,
                        (e, t_join) => new { Employee = e, Tvb = t_join.DefaultIfEmpty() }
                    )
                    .SelectMany(
                        et => et.Tvb,
                        (et, tvb) => new EmployeeFinancials
                        {
                            BadgeNumber = et.Employee.BadgeNumber,
                            Ssn = et.Employee.Ssn,
                            Name = et.Employee.Name,
                            EnrolledId = et.Employee.EnrolledId,
                            YearsInPlan = et.Employee.YearsInPlan,
                            CurrentAmount = tvb == null ? 0 : tvb.CurrentBalance,
                            EmployeeTypeId = et.Employee.EmployeeTypeId,
                            PointsEarned = et.Employee.PointsEarned,
                            /* This value of ETVA is not the users actual ETVA for about 1/3 for all members in the obfuscated dataset.
                               This problem will need to be addressed in the future.
                               You can see this if you run the Integration test for TotalServices.
                                */
                            Etva = tvb == null ? 0 : tvb.Etva,
                            ZeroContributionReasonId = et.Employee.ZeroContributionReasonId
                        }
                    )
                    .ToList();
        });

        foreach (EmployeeFinancials empl in employeeFinancialsList)
        {
            // if employee is not participating 
            if (empl.EnrolledId != Enrollment.Constants.NotEnrolled || empl.YearsInPlan != 0)
            {
                var (memb, didEmployeeExceededMaxContribution) = await ProcessEmployee(empl, profitShareUpdateRequest, adjustmentReportData, cancellationToken);
                members.Add(memb);
                employeeExceededMaxContribution |= didEmployeeExceededMaxContribution;
            }
        }

        return employeeExceededMaxContribution;
    }

    private async Task ProcessBeneficiaries(List<MemberFinancials> members, ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        List<BeneficiaryFinancials> benes = await _dbContextFactory.UseReadOnlyContext(ctx =>
            ctx.Beneficiaries.OrderBy(b => b.Contact!.ContactInfo.FullName)
                .ThenByDescending(b => b.BadgeNumber * 10000 + b.PsnSuffix).Select(b =>
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

            MemberFinancials memb = await ProcessBeneficiary(bene, profitShareUpdateRequest, cancellationToken);
            if (!memb.IsAllZeros())
            {
                members.Add(memb);
            }
        }
    }

    private async Task<(MemberFinancials, bool)> ProcessEmployee(EmployeeFinancials empl, ProfitShareUpdateRequest profitShareUpdateRequest,
        AdjustmentReportData adjustmentReportData, CancellationToken cancellationToken)
    {
        // Gets this year's profit sharing transactions, aka Distributions - hardships - Military - ClassActionFund
        ProfitDetailTotals profitDetailTotals =
            await ProfitDetailTotals.GetProfitDetailTotals(_dbContextFactory, (short)(profitShareUpdateRequest.ProfitYear -1), empl.Ssn, cancellationToken);

        // MemberTotals holds newly computed values, not old values
        MemberTotals memberTotals = new();

        memberTotals.ContributionAmount =
            ComputeContribution(empl.PointsEarned, empl.BadgeNumber, profitShareUpdateRequest, adjustmentReportData);
        memberTotals.IncomingForfeitureAmount =
            ComputeForfeitures(empl.PointsEarned, empl.BadgeNumber, profitShareUpdateRequest, adjustmentReportData);

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

        ComputeEarningsEmployee(empl, memberTotals, profitShareUpdateRequest, adjustmentReportData, profitDetailTotals.ClassActionFundTotal);

        MemberFinancials memberFinancials = new(empl, profitDetailTotals, memberTotals);

        //   --- Max Contribution Concerns --- 
        decimal memberTotalContribution = memberTotals.ContributionAmount + profitDetailTotals.MilitaryTotal +
                                          memberTotals.IncomingForfeitureAmount;

        bool employeeExceededMaxContribution = false;
        if (memberTotalContribution > profitShareUpdateRequest.MaxAllowedContributions)
        {
            decimal overContribution = memberTotalContribution - profitShareUpdateRequest.MaxAllowedContributions;

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

    private async Task<MemberFinancials> ProcessBeneficiary(BeneficiaryFinancials bene, ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        var thisYearsTotals =
            await ProfitDetailTotals.GetProfitDetailTotals(_dbContextFactory, profitShareUpdateRequest.ProfitYear, bene.Ssn, cancellationToken);

        MemberTotals memberTotals = new();
        // Yea, this adding and removing ClassActionFundTotal is strange
        memberTotals.NewCurrentAmount = thisYearsTotals.AllocationsTotal + thisYearsTotals.ClassActionFundTotal +
                                        (bene.CurrentAmount - thisYearsTotals.ForfeitsTotal -
                                         thisYearsTotals.PaidAllocationsTotal) -
                                        thisYearsTotals.DistributionsTotal;
        memberTotals.NewCurrentAmount -= thisYearsTotals.ClassActionFundTotal;

        if (memberTotals.NewCurrentAmount > 0)
        {
            memberTotals.PointsDollars = Math.Round(memberTotals.NewCurrentAmount, 2, MidpointRounding.AwayFromZero);
            memberTotals.EarnPoints = (int)Math.Round(memberTotals.PointsDollars / 100, MidpointRounding.AwayFromZero);
        }

        ComputeEarningsBeneficiary(memberTotals, bene, profitShareUpdateRequest);

        return new MemberFinancials(bene, thisYearsTotals, memberTotals);
    }


    private static decimal ComputeContribution(long pointsEarned, long badge, ProfitShareUpdateRequest profitShareUpdateRequest,
        AdjustmentReportData adjustmentReportData)
    {
        decimal contributionAmount = Math.Round(profitShareUpdateRequest.ContributionPercent * pointsEarned, 2,
            MidpointRounding.AwayFromZero);

        if (profitShareUpdateRequest.BadgeToAdjust > 0 && profitShareUpdateRequest.BadgeToAdjust == badge)
        {
            adjustmentReportData.ContributionAmountUnadjusted = contributionAmount;
            contributionAmount += profitShareUpdateRequest.AdjustContributionAmount;
            adjustmentReportData.ContributionAmountAdjusted = contributionAmount;
        }

        return contributionAmount;
    }


    private static decimal ComputeForfeitures(long pointsEarned, long badge, ProfitShareUpdateRequest profitShareUpdateRequest,
        AdjustmentReportData adjustmentReportData)
    {
        decimal incomingForfeitureAmount = Math.Round(profitShareUpdateRequest.IncomingForfeitPercent * pointsEarned, 2, MidpointRounding.AwayFromZero);
        if (profitShareUpdateRequest.BadgeToAdjust > 0 && profitShareUpdateRequest.BadgeToAdjust == badge)
        {
            adjustmentReportData.IncomingForfeitureAmountUnadjusted = incomingForfeitureAmount;
            incomingForfeitureAmount += profitShareUpdateRequest.AdjustIncomingForfeitAmount;
            adjustmentReportData.IncomingForfeitureAmountAdjusted = incomingForfeitureAmount;
        }

        return incomingForfeitureAmount;
    }

    // The fact that this method takes either a bene or an empl and has all this conditional logic is not great.
    private static void ComputeEarningsEmployee(EmployeeFinancials empl, MemberTotals memberTotals, ProfitShareUpdateRequest profitShareUpdateRequest,
        AdjustmentReportData? adjustmentsApplied, decimal classActionFundTotal)
    {
        if (memberTotals.EarnPoints <= 0)
        {
            memberTotals.EarnPoints = 0;
            empl.Earnings = 0;
            empl.SecondaryEarnings = 0;
        }

        memberTotals.EarningsAmount = Math.Round(profitShareUpdateRequest.EarningsPercent * memberTotals.EarnPoints, 2,
            MidpointRounding.AwayFromZero);
        if (profitShareUpdateRequest.BadgeToAdjust > 0 && profitShareUpdateRequest.BadgeToAdjust == (empl?.BadgeNumber ?? 0))
        {
            adjustmentsApplied!.EarningsAmountUnadjusted = memberTotals.EarningsAmount;
            memberTotals.EarningsAmount += profitShareUpdateRequest.AdjustEarningsAmount;
            adjustmentsApplied.EarningsAmountAdjusted = memberTotals.EarningsAmount;
        }

        memberTotals.SecondaryEarningsAmount =
            Math.Round(profitShareUpdateRequest.SecondaryEarningsPercent * memberTotals.EarnPoints, 2,
                MidpointRounding.AwayFromZero);
        if (profitShareUpdateRequest.BadgeToAdjust2 > 0 && profitShareUpdateRequest.BadgeToAdjust2 == (empl?.BadgeNumber ?? 0))
        {
            adjustmentsApplied!.SecondaryEarningsAmountUnadjusted = memberTotals.SecondaryEarningsAmount;
            memberTotals.SecondaryEarningsAmount += profitShareUpdateRequest.AdjustEarningsSecondaryAmount;
            adjustmentsApplied.SecondaryEarningsAmountAdjusted = memberTotals.SecondaryEarningsAmount;
        }


        decimal workingEtva = 0;
        // When the CAF is present and the memeber is under 6 in YIP, we need to remove the CAF from the ETVA for earnings calculations.
        // Need to subtract CAF out of PY-PS-ETVA (ETVA) for people not fully vested
        // because we can't give earnings for 2021 on class action funds -
        // they were added in 2021.  CAF was added to PY-PS-ETVA (ETVA) for
        // PY-PS-YEARS < 6.
        if (empl!.Etva > 0)
        {
            // This check for 6 years is only used here, so it is intentionally not pulled out.
            // It is presumed that this 6 is specific to the 2021 adjustment.   It is assumed that the
            // plan enrollment type is specifically not consulted (i.e. No OLD vs NEW plan)
            if (empl.YearsInPlan < 6)
            {
                workingEtva = empl.Etva - classActionFundTotal;
            }
            else
            {
                empl.Etva = workingEtva;
            }
        }

        if (workingEtva <= 0)
        {
            empl.Earnings = memberTotals.EarningsAmount; // set PY-PROF-EARN
            empl.SecondaryEarnings = memberTotals.SecondaryEarningsAmount; // set PY-PROF-EARN2
            empl.EarningsOnEtva = 0m;
            empl.EarningsOnSecondaryEtva = 0m;
            return;
        }

        if (memberTotals.PointsDollars <= 0)
        {
            return;
        }

        // Here we scale the Earned interest to report on what portion is in a 0 record (contribution), vs what goes in an 8 record (100% ETVA Earnings) 
        decimal etvaScaled = workingEtva / memberTotals.PointsDollars;

        // Sets Earn and ETVA amounts
        empl!.Earnings = memberTotals.EarningsAmount;
        empl.EarningsOnEtva = Math.Round(memberTotals.EarningsAmount * etvaScaled, 2, MidpointRounding.AwayFromZero);

        empl.SecondaryEarnings = memberTotals.SecondaryEarningsAmount;
        empl.EarningsOnSecondaryEtva = Math.Round(memberTotals.SecondaryEarningsAmount * etvaScaled, 2, MidpointRounding.AwayFromZero);
    }

    private static void ComputeEarningsBeneficiary(MemberTotals memberTotals, BeneficiaryFinancials bene, ProfitShareUpdateRequest profitShareUpdateRequest)
    {
        memberTotals.EarningsAmount = Math.Round(profitShareUpdateRequest.EarningsPercent * memberTotals.EarnPoints, 2,
            MidpointRounding.AwayFromZero);

        bene!.Earnings = memberTotals.EarningsAmount;

        memberTotals.SecondaryEarningsAmount =
            Math.Round(profitShareUpdateRequest.SecondaryEarningsPercent * memberTotals.EarnPoints, 2,
                MidpointRounding.AwayFromZero);

        bene.SecondaryEarnings = memberTotals.SecondaryEarningsAmount;
    }
}
