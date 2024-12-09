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
    private readonly ILogger<ProfitShareUpdateService> _logger;
    private readonly TotalService _totalService;

    // Indicates that MaxContributions were exceeded.   Adjustments need be made and the update should be rerun.
    // TBD This needs to be evolved to return the list of employees who have exceeded the max contribution.
    private bool _rerunNeeded;

    public ProfitShareUpdateService(IProfitSharingDataContextFactory dbContextFactory, ILoggerFactory loggerFactory, ICalendarService calendarService)
    {
        _dbContextFactory = dbContextFactory;
        _totalService = new TotalService(dbContextFactory, calendarService);
        _logger = loggerFactory.CreateLogger<ProfitShareUpdateService>();
        _calendarService = calendarService;
    }


    public async Task<ReportResponseBase<MemberFinancialsResponse>> ApplyAdjustmentsPaginated(UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest, CancellationToken cancellationToken)
    {
        var outcome = await ApplyAdjustments(updateAdjustmentAmountsRequest);
        var members = outcome.Item1.Select(m => new MemberFinancialsResponse
        {
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

        return new ReportResponseBase<MemberFinancialsResponse>
        {
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
    /// <param name="updateAdjustmentAmountsRequest"></param>
    /// <returns>
    ///     member financials - a summary of members who have been updated
    ///     adjustments applied - the before and after values for a single adjusted badge
    ///     bool - true indicates that one or more employees over the max contribution for the year
    /// </returns>
    public async Task<(List<MemberFinancials>, AdjustmentsApplied, bool)> ApplyAdjustments(
        UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest)
    {
        // Values collected for an "Adjustment Report" that we do not yet generate
        AdjustmentsApplied adjustmentsApplied = new();

        List<MemberFinancials> members = new();
        await ProcessEmployees(members, updateAdjustmentAmountsRequest, adjustmentsApplied);
        await ProcessBeneficiaries(members, updateAdjustmentAmountsRequest);

        foreach (MemberFinancials memberFinancials in members)
        {
            memberFinancials.EndingBalance = memberFinancials.CurrentAmount + memberFinancials.Contributions +
                                             memberFinancials.Xfer - memberFinancials.Pxfer +
                                             memberFinancials.Earnings + memberFinancials.SecondaryEarnings +
                                             memberFinancials.IncomingForfeitures + memberFinancials.Military +
                                             memberFinancials.Caf -
                                             memberFinancials.Distributions;
        }

        return (members, adjustmentsApplied, _rerunNeeded);
    }

    public async Task ProcessEmployees(List<MemberFinancials> members, UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest,
        AdjustmentsApplied adjustmentsApplied)
    {
        var fiscalDates = await _calendarService.GetYearStartAndEndAccountingDatesAsync(updateAdjustmentAmountsRequest.ProfitYear, CancellationToken.None);
        List<EmployeeFinancials> employeeFinancialsList = await _dbContextFactory.UseReadOnlyContext(async ctx =>
        {
            IQueryable<ParticipantTotalVestingBalanceDto> totalVestingBalances =
                _totalService.TotalVestingBalance(ctx, (short)(updateAdjustmentAmountsRequest.ProfitYear - 1), fiscalDates.FiscalEndDate);

            return await ctx.PayProfits
                .Include(pp => pp.Demographic)
                .Where(pp => pp.ProfitYear == updateAdjustmentAmountsRequest.ProfitYear - 1)
                .Join(
                    totalVestingBalances,
                    pp => pp.Demographic!.Ssn,
                    tvb => tvb.Ssn,
                    (pp, tvb) => new EmployeeFinancials
                    {
                        EmployeeId = pp.Demographic!.EmployeeId,
                        Ssn = pp.Demographic.Ssn,
                        Name = pp.Demographic.ContactInfo!.FullName,
                        EnrolledId = pp.EnrollmentId,
                        YearsInPlan = pp.YearsInPlan,
                        CurrentAmount = tvb.CurrentBalance,
                        EmployeeTypeId = pp.EmployeeTypeId,
                        PointsEarned = (long)pp.PointsEarned!,
                        EtvaAfterVestingRules = tvb.Etva
                    }
                )
                .OrderBy(ef => ef.EmployeeId)
                .ToListAsync();
        });

        foreach (EmployeeFinancials empl in employeeFinancialsList)
        {
            // if employee is not participating 
            if (empl.EnrolledId == Enrollment.Constants.NotEnrolled && empl.YearsInPlan == 0)
            {                
                continue;
            }

            MemberFinancials memb = await ProcessEmployee(empl, updateAdjustmentAmountsRequest, adjustmentsApplied);
            members.Add(memb);
        }
    }

    private async Task ProcessBeneficiaries(List<MemberFinancials> members, UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest)
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
                    }).ToListAsync()
        );

        foreach (BeneficiaryFinancials bene in benes)
        {
            // is already handled as an employee?
            if (members.Any(m => m.Ssn == bene.Ssn))
            {
                continue;
            }

            MemberFinancials memb = await ProcessBeneficiary(bene, updateAdjustmentAmountsRequest);
            members.Add(memb);
        }
    }

    public async Task<MemberFinancials> ProcessEmployee(EmployeeFinancials empl, UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest,
        AdjustmentsApplied adjustmentsApplied)
    {

        // Gets this years profit sharing transactions, aka Distributions - hardships
        DetailTotals detailTotals = await GetDetailTotals(updateAdjustmentAmountsRequest.ProfitYear, empl.Ssn);

        // MemberTotals holds newly computed values, not old values
        MemberTotals memberTotals = new();

        memberTotals.ContributionAmount =
            ComputeContribution(empl.PointsEarned, empl.EmployeeId, updateAdjustmentAmountsRequest, adjustmentsApplied);
        memberTotals.IncomingForfeitureAmount =
            ComputeForfeitures(empl.PointsEarned, empl.EmployeeId, updateAdjustmentAmountsRequest, adjustmentsApplied);

        // This "EarningsBalance" is actually the new Current Balance.  Consider changing the name
        // Note that CAF gets added here, but subtracted in the next line.   Odd.
        memberTotals.NewCurrentAmount = detailTotals.AllocationsTotal + detailTotals.ClassActionFundTotal +
                                        (empl.CurrentAmount - detailTotals.ForfeitsTotal -
                                         detailTotals.PaidAllocationsTotal) -
                                        detailTotals.DistributionsTotal;
        memberTotals.NewCurrentAmount -= detailTotals.ClassActionFundTotal;

        if (memberTotals.NewCurrentAmount <= 0)
        {
            memberTotals.EarnPoints = 0;
            memberTotals.PointsDollars = 0;
        }
        else
        {
            memberTotals.PointsDollars = Math.Round(memberTotals.NewCurrentAmount, 2, MidpointRounding.AwayFromZero);
            memberTotals.EarnPoints = (long)Math.Round(memberTotals.PointsDollars / 100, MidpointRounding.AwayFromZero);
        }

        ComputeEarnings(memberTotals, null, empl, updateAdjustmentAmountsRequest, adjustmentsApplied,
            detailTotals.ClassActionFundTotal);

        MemberFinancials memberFinancials = new();
        memberFinancials.EmployeeId = empl.EmployeeId;
        memberFinancials.Psn = empl.EmployeeId;
        memberFinancials.Name = empl.Name;
        memberFinancials.Ssn = empl.Ssn;
        memberFinancials.Xfer = detailTotals.AllocationsTotal;
        memberFinancials.Pxfer = detailTotals.PaidAllocationsTotal;
        memberFinancials.CurrentAmount = empl.CurrentAmount;
        memberFinancials.Distributions = detailTotals.DistributionsTotal;
        memberFinancials.Military = detailTotals.MilitaryTotal;
        memberFinancials.Caf = detailTotals.ClassActionFundTotal;
        memberFinancials.EmployeeTypeId = empl.EmployeeTypeId;
        memberFinancials.ContributionPoints = empl.PointsEarned;
        memberFinancials.EarningPoints = memberTotals.EarnPoints;
        memberFinancials.Contributions = memberTotals.ContributionAmount;
        memberFinancials.IncomingForfeitures = memberTotals.IncomingForfeitureAmount;
        memberFinancials.IncomingForfeitures -= detailTotals.ForfeitsTotal;
        memberFinancials.Earnings = empl.Earnings;
        memberFinancials.Earnings += empl.EarningsOnEtva;
        memberFinancials.SecondaryEarnings = empl.SecondaryEarnings;
        memberFinancials.SecondaryEarnings += empl.SecondaryEtvaEarnings;

        //   --- Max Contribution Concerns --- 
        decimal memberTotalContribution = memberTotals.ContributionAmount + detailTotals.MilitaryTotal +
                                          memberTotals.IncomingForfeitureAmount;

        if (memberTotalContribution > updateAdjustmentAmountsRequest.MaxAllowedContributions)
        {
            decimal overContribution = memberTotalContribution - updateAdjustmentAmountsRequest.MaxAllowedContributions;

            if (overContribution < memberTotals.IncomingForfeitureAmount)
            {
                memberFinancials.IncomingForfeitures -= overContribution;
            }
            else
            {
                _logger.LogError("FORFEITURES NOT ENOUGH FOR AMOUNT OVER MAX FOR EMPLOYEE BADGE {EmployeeId}", empl.EmployeeId);
                memberFinancials.IncomingForfeitures = 0;
            }

            memberFinancials.MaxOver = overContribution;
            memberFinancials.MaxPoints = memberFinancials.ContributionPoints;
            _rerunNeeded = true;
        }
        // --- End Max Contribution

        empl.Contributions = memberTotals.ContributionAmount;
        empl.IncomeForfeiture = memberTotals.IncomingForfeitureAmount;
        return memberFinancials;
    }


    public async Task<MemberFinancials> ProcessBeneficiary(BeneficiaryFinancials bene, UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest)
    {
        DetailTotals detailTotals = await GetDetailTotals(updateAdjustmentAmountsRequest.ProfitYear, bene.Ssn);

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
            memberTotals.EarnPoints = (long)Math.Round(memberTotals.PointsDollars / 100, MidpointRounding.AwayFromZero);
        }

        ComputeEarnings(memberTotals, bene, null, updateAdjustmentAmountsRequest, null, detailTotals.ClassActionFundTotal);

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


    private static decimal ComputeContribution(long PointsEarned, long badge, UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest,
        AdjustmentsApplied adjustmentsApplied)
    {
        decimal contributionAmount = Math.Round(updateAdjustmentAmountsRequest.ContributionPercent * PointsEarned, 2,
            MidpointRounding.AwayFromZero);

        if (updateAdjustmentAmountsRequest.BadgeToAdjust > 0 && updateAdjustmentAmountsRequest.BadgeToAdjust == badge)
        {
            adjustmentsApplied.ContributionAmountUnadjusted = contributionAmount;
            contributionAmount += updateAdjustmentAmountsRequest.AdjustContributionAmount;
            adjustmentsApplied.ContributionAmountAdjusted = contributionAmount;
        }

        return contributionAmount;
    }


    private static decimal ComputeForfeitures(long PointsEarned, long badge, UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest,
        AdjustmentsApplied adjustmentsApplied)
    {
        decimal incomingForfeitureAmount = Math.Round(updateAdjustmentAmountsRequest.IncomingForfeitPercent * PointsEarned, 2,
            MidpointRounding.AwayFromZero);
        if (updateAdjustmentAmountsRequest.BadgeToAdjust > 0 && updateAdjustmentAmountsRequest.BadgeToAdjust == badge)
        {
            adjustmentsApplied.IncomingForfeitureAmountUnadjusted = incomingForfeitureAmount;
            incomingForfeitureAmount += updateAdjustmentAmountsRequest.AdjustIncomingForfeitAmount;
            adjustmentsApplied.IncomingForfeitureAmountAdjusted = incomingForfeitureAmount;
        }

        return incomingForfeitureAmount;
    }

    // The fact that this method takes either a bene or an empl and has all this conditional logic is not great.
    public static void ComputeEarnings(MemberTotals memberTotals, BeneficiaryFinancials? bene, EmployeeFinancials? empl,
        UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest, AdjustmentsApplied? adjustmentsApplied, decimal ClassActionFundTotal)
    {
        if (memberTotals.EarnPoints <= 0 && empl != null)
        {
            memberTotals.EarnPoints = 0;
            empl.Earnings = 0;
            empl.SecondaryEarnings = 0;
        }

        memberTotals.EarningsAmount = Math.Round(updateAdjustmentAmountsRequest.EarningsPercent * memberTotals.EarnPoints, 2,
            MidpointRounding.AwayFromZero);
        if (updateAdjustmentAmountsRequest.BadgeToAdjust > 0 && updateAdjustmentAmountsRequest.BadgeToAdjust == (empl?.EmployeeId ?? 0))
        {
            adjustmentsApplied!.EarningsAmountUnadjusted = memberTotals.EarningsAmount;
            memberTotals.EarningsAmount += updateAdjustmentAmountsRequest.AdjustEarningsAmount;
            adjustmentsApplied.EarningsAmountAdjusted = memberTotals.EarningsAmount;
        }

        memberTotals.SecondaryEarningsAmount =
            Math.Round(updateAdjustmentAmountsRequest.SecondaryEarningsPercent * memberTotals.EarnPoints, 2,
                MidpointRounding.AwayFromZero);
        if (updateAdjustmentAmountsRequest.BadgeToAdjust2 > 0 && updateAdjustmentAmountsRequest.BadgeToAdjust2 == (empl?.EmployeeId ?? 0))
        {
            adjustmentsApplied!.SecondaryEarningsAmountUnadjusted = memberTotals.SecondaryEarningsAmount;
            memberTotals.SecondaryEarningsAmount += updateAdjustmentAmountsRequest.AdjustEarningsSecondaryAmount;
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

        if (updateAdjustmentAmountsRequest.SecondaryEarningsPercent != 0m) // Secondary Earnings
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
    public async Task<DetailTotals> GetDetailTotals(short profitYear, int ssn)
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
                .ToListAsync()
        );

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

