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
        var (memberFinancials, _, employeeExceededMaxContribution) = await ProfitSharingUpdatePaginated(profitSharingUpdateRequest, cancellationToken);
        var members = memberFinancials.Select(m => new MemberFinancialsResponse
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

        return new ProfitShareUpdateResponse {
            IsReRunRequired = employeeExceededMaxContribution,
            ReportName = "Profit Sharing Update",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<MemberFinancialsResponse>
            {
                Results = members
            }
        };
    }

    /// <summary>
    ///     Apply updates to profit sharing system.
    /// </summary>
    /// <param name="profitSharingUpdateRequest"></param>
    /// <returns>
    ///     member financials - a summary of members who have been updated
    ///     adjustments applied - the before and after values for a single adjusted badge
    ///     bool - true indicates that one or more employees over the max contribution for the year
    /// </returns>
    public async Task<ProfitShareUpdateOutcome> ProfitSharingUpdatePaginated(ProfitSharingUpdateRequest profitSharingUpdateRequest, CancellationToken cancellationToken)
    {
        // Values collected for an "Adjustment Report" that we do not yet generate
        AdjustmentReportData adjustmentReportData = new();

        List<MemberFinancials> members = new();
        bool employeeExceededMaxContribution = await ProcessEmployees(members, profitSharingUpdateRequest, adjustmentReportData, cancellationToken);
        await ProcessBeneficiaries(members, profitSharingUpdateRequest, cancellationToken);

        foreach (MemberFinancials memberFinancials in members)
        {
            memberFinancials.EndingBalance = memberFinancials.CurrentAmount + memberFinancials.Contributions +
                                             memberFinancials.Xfer - memberFinancials.Pxfer +
                                             memberFinancials.Earnings + memberFinancials.SecondaryEarnings +
                                             memberFinancials.IncomingForfeitures + memberFinancials.Military +
                                             memberFinancials.Caf -
                                             memberFinancials.Distributions;
        }

        return new (members, adjustmentReportData, employeeExceededMaxContribution);
    }

    private async Task<bool> ProcessEmployees(List<MemberFinancials> members, ProfitSharingUpdateRequest profitSharingUpdateRequest,
        AdjustmentReportData adjustmentReportData, CancellationToken cancellationToken)
    {
        var employeeExceededMaxContribution = false;
        var fiscalDates = await _calendarService.GetYearStartAndEndAccountingDatesAsync(profitSharingUpdateRequest.ProfitYear, cancellationToken);
        List<EmployeeFinancials> employeeFinancialsList = await _dbContextFactory.UseReadOnlyContext(async ctx =>
        {
            IQueryable<ParticipantTotalVestingBalanceDto> totalVestingBalances =
                ((TotalService)_totalService).TotalVestingBalance(ctx,
                    (short)(profitSharingUpdateRequest.ProfitYear - 1), fiscalDates.FiscalEndDate);

            return await ctx.PayProfits
                .Include(pp => pp.Demographic)
                .Include(pp => pp.Demographic!.ContactInfo)
                .Where(pp => pp.ProfitYear == profitSharingUpdateRequest.ProfitYear)
                .GroupJoin(
                    totalVestingBalances,
                    pp => pp.Demographic!.Ssn,
                    tvb => tvb.Ssn,
                    (pp, tvbs) => new { PayProfit = pp, TotalVestingBalances = tvbs.DefaultIfEmpty() }
                )
                .SelectMany(
                    x => x.TotalVestingBalances,
                    (x, tvb) => new EmployeeFinancials
                    {
                        EmployeeId = x.PayProfit.Demographic!.EmployeeId,
                        Ssn = x.PayProfit.Demographic.Ssn,
                        Name = x.PayProfit.Demographic.ContactInfo!.FullName,
                        EnrolledId = x.PayProfit.EnrollmentId,
                        YearsInPlan = x.PayProfit.YearsInPlan,
                        CurrentAmount = tvb == null ? 0 : tvb.CurrentBalance,
                        EmployeeTypeId = x.PayProfit.EmployeeTypeId,
                        PointsEarned = (int)(x.PayProfit.PointsEarned ?? 0),
                        EtvaAfterVestingRules = tvb == null ? 0 : tvb.Etva
                    }
                )
                .ToListAsync(cancellationToken);
        });

        foreach (EmployeeFinancials empl in employeeFinancialsList)
        {
            // if employee is not participating 
            if (empl.EnrolledId != Enrollment.Constants.NotEnrolled || empl.YearsInPlan != 0)
            {
                var ( memb, didEmployeeExceededMaxContribution ) = await ProcessEmployee(empl, profitSharingUpdateRequest, adjustmentReportData, cancellationToken);
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
                        CurrentAmount = b.Amount,
                        Earnings = b.Earnings,
                        SecondaryEarnings = b.SecondaryEarnings
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
            members.Add(memb);
        }
    }

    private async Task<(MemberFinancials, bool)> ProcessEmployee(EmployeeFinancials empl, ProfitSharingUpdateRequest profitSharingUpdateRequest,
        AdjustmentReportData adjustmentReportData, CancellationToken cancellationToken)
    {

        // Gets this years profit sharing transactions, aka Distributions - hardships
        DetailTotals detailTotals = await GetDetailTotals(profitSharingUpdateRequest.ProfitYear, empl.Ssn, cancellationToken);

        // MemberTotals holds newly computed values, not old values
        MemberTotals memberTotals = new();

        memberTotals.ContributionAmount =
            ComputeContribution(empl.PointsEarned, empl.EmployeeId, profitSharingUpdateRequest, adjustmentReportData);
        memberTotals.IncomingForfeitureAmount =
            ComputeForfeitures(empl.PointsEarned, empl.EmployeeId, profitSharingUpdateRequest, adjustmentReportData);

        // This "EarningsBalance" is actually the new Current Balance.  Consider changing the name
        // Note that CAF gets added here, but subtracted in the next line.   Odd.
        memberTotals.NewCurrentAmount = detailTotals.AllocationsTotal + detailTotals.ClassActionFundTotal +
                                        (empl.CurrentAmount - detailTotals.ForfeitsTotal -
                                         detailTotals.PaidAllocationsTotal) -
                                        detailTotals.DistributionsTotal;
        memberTotals.NewCurrentAmount -= detailTotals.ClassActionFundTotal;

        if (memberTotals.NewCurrentAmount > 0)
        {
            memberTotals.PointsDollars = Math.Round(memberTotals.NewCurrentAmount, 2, MidpointRounding.AwayFromZero);
            memberTotals.EarnPoints = (int)Math.Round(memberTotals.PointsDollars / 100, MidpointRounding.AwayFromZero);
        }

        ComputeEarnings(memberTotals, null, empl, profitSharingUpdateRequest, adjustmentReportData,
            detailTotals.ClassActionFundTotal);

        MemberFinancials memberFinancials = new(empl, detailTotals, memberTotals);

        //   --- Max Contribution Concerns --- 
        decimal memberTotalContribution = memberTotals.ContributionAmount + detailTotals.MilitaryTotal +
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
        DetailTotals detailTotals = await GetDetailTotals(profitSharingUpdateRequest.ProfitYear, bene.Ssn, cancellationToken);

        MemberTotals memberTotals = new();
        // Yea, this adding and removing ClassActionFundTotal is strange
        memberTotals.NewCurrentAmount = detailTotals.AllocationsTotal + detailTotals.ClassActionFundTotal +
                                        (bene.CurrentAmount - detailTotals.ForfeitsTotal -
                                         detailTotals.PaidAllocationsTotal) -
                                        detailTotals.DistributionsTotal;
        memberTotals.NewCurrentAmount -= detailTotals.ClassActionFundTotal;

        if (memberTotals.NewCurrentAmount > 0)
        {
            memberTotals.PointsDollars = Math.Round(memberTotals.NewCurrentAmount, 2, MidpointRounding.AwayFromZero);
            memberTotals.EarnPoints = (int)Math.Round(memberTotals.PointsDollars / 100, MidpointRounding.AwayFromZero);
        }

        ComputeEarnings(memberTotals, bene, null, profitSharingUpdateRequest, null, detailTotals.ClassActionFundTotal);

        return new MemberFinancials(bene, detailTotals, memberTotals);
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


    private static decimal ComputeForfeitures(long PointsEarned, long badge, ProfitSharingUpdateRequest profitSharingUpdateRequest,
        AdjustmentReportData adjustmentReportData)
    {
        decimal incomingForfeitureAmount = Math.Round(profitSharingUpdateRequest.IncomingForfeitPercent * PointsEarned, 2,
            MidpointRounding.AwayFromZero);
        if (profitSharingUpdateRequest.BadgeToAdjust > 0 && profitSharingUpdateRequest.BadgeToAdjust == badge)
        {
            adjustmentReportData.IncomingForfeitureAmountUnadjusted = incomingForfeitureAmount;
            incomingForfeitureAmount += profitSharingUpdateRequest.AdjustIncomingForfeitAmount;
            adjustmentReportData.IncomingForfeitureAmountAdjusted = incomingForfeitureAmount;
        }

        return incomingForfeitureAmount;
    }

    // The fact that this method takes either a bene or an empl and has all this conditional logic is not great.
    private static void ComputeEarnings(MemberTotals memberTotals, BeneficiaryFinancials? bene, EmployeeFinancials? empl,
        ProfitSharingUpdateRequest profitSharingUpdateRequest, AdjustmentReportData? adjustmentsApplied, decimal ClassActionFundTotal)
    {
        if (memberTotals.EarnPoints <= 0 && empl != null)
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

        // This comment from cobol helps explain the following if block.  It is related to CAF processing.
        //* -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        //* ETVA EARNINGS ARE CALCULATED AND WRITTEN TO PY-PROF-ETVA (EtvaAfterVestingRules)
        //* -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        //* need to subtract CAF out of PY-PS-ETVA (EtvaAfterVestingRules) for people not fully vested
        //* because  we can't give earnings for 2021 on class action funds -
        //* they were added in 2021.CAF was added to PY-PS-ETVA (EtvaAfterVestingRules) for
        //* PY-PS-YEARS < 6.

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

        if (profitSharingUpdateRequest.SecondaryEarningsPercent != 0m) // Secondary Earnings
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

    // Fetches PROFIT_DETAIL Totals for an SSN.
    // processes only the current profit year.  Ignores profit code = 0.
    // Special handling for CAF and Military. 
    private async Task<DetailTotals> GetDetailTotals(short profitYear, int ssn, CancellationToken cancellationToken)
    {
        decimal distributionsTotal = 0;
        decimal forfeitsTotal = 0;
        decimal allocationsTotal = 0;
        decimal paidAllocationsTotal = 0;
        decimal militaryTotal = 0;
        decimal classActionFundTotal = 0;

        List<ProfitDetail> pds = await _dbContextFactory.UseReadOnlyContext(ctx =>
            ctx.ProfitDetails.Where(pd => pd.Ssn == ssn && pd.ProfitYear == profitYear)
                .OrderBy(pd => pd.ProfitYear).ThenBy(pd => pd.ProfitYearIteration).ThenBy(pd => pd.MonthToDate)
                .ThenBy(pd => pd.FederalTaxes)
                .ToListAsync(cancellationToken)
        );

        foreach (ProfitDetail pd in pds)
        {
            if (pd.ProfitCodeId == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal /*1*/ ||
                pd.ProfitCodeId == ProfitCode.Constants.OutgoingDirectPayments /*3*/)
            {
                distributionsTotal += pd.Forfeiture;
            }

            if (pd.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment /*9*/)
            {
                if (pd.CommentType == CommentType.Constants.TransferOut /* "XFER >" or "XFER>" */  ||
                    pd.CommentType == CommentType.Constants.QdroOut /* "QDRO >" or "QDRO>" */)
                {
                    paidAllocationsTotal += pd.Forfeiture;
                }
                else
                {
                    distributionsTotal += pd.Forfeiture;
                }
            }

            if (pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures /*2*/)
            {
                forfeitsTotal += pd.Forfeiture;
            }

            if (pd.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary /*5*/)
            {
                paidAllocationsTotal += pd.Forfeiture;
            }

            if (pd.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary /*6*/)
            {
                allocationsTotal += pd.Contribution;
            }

            if (pd.ProfitYearIteration == ProfitDetail.Constants.ProfitYearIterationMilitary /*1*/)
            {
                militaryTotal += pd.Contribution;
            }

            if (pd.ProfitYearIteration == ProfitDetail.Constants.ProfitYearIterationClassActionFund /*2*/)
            {
                classActionFundTotal += pd.Earnings;
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

