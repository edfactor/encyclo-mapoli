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
    private readonly ILogger<ProfitShareUpdateService> _logger;
    private readonly TotalService _totalService;

    public ProfitShareUpdateService(IProfitSharingDataContextFactory dbContextFactory, ILoggerFactory loggerFactory, TotalService totalService, ICalendarService calendarService)
    {
        _dbContextFactory = dbContextFactory;
        _totalService = totalService;
        _logger = loggerFactory.CreateLogger<ProfitShareUpdateService>();
        _calendarService = calendarService;
    }


    public async Task<ProfitShareUpdateResponse> ApplyAdjustmentsPaginated(UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest, CancellationToken cancellationToken)
    {
        var (memberFinancials, _, isReRunRequired) = await ApplyAdjustments(updateAdjustmentAmountsRequest, cancellationToken);
        var members = memberFinancials.Select(m => new MemberFinancialsResponse
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

        return new ProfitShareUpdateResponse {
            IsReRunRequired = isReRunRequired,
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
    public async Task<ProfitShareUpdateOutcome> ApplyAdjustments(UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest, CancellationToken cancellationToken)
    {
        // Values collected for an "Adjustment Report" that we do not yet generate
        AdjustmentReportData adjustmentReportData = new();

        List<MemberFinancials> members = new();
        bool rerunNeeded = await ProcessEmployees(members, updateAdjustmentAmountsRequest, adjustmentReportData, cancellationToken);
        await ProcessBeneficiaries(members, updateAdjustmentAmountsRequest, cancellationToken);

        foreach (MemberFinancials memberFinancials in members)
        {
            memberFinancials.EndingBalance = memberFinancials.CurrentAmount + memberFinancials.Contributions +
                                             memberFinancials.Xfer - memberFinancials.Pxfer +
                                             memberFinancials.Earnings + memberFinancials.SecondaryEarnings +
                                             memberFinancials.IncomingForfeitures + memberFinancials.Military +
                                             memberFinancials.Caf -
                                             memberFinancials.Distributions;
        }

        return new (members, adjustmentReportData, rerunNeeded);
    }

    private async Task<bool> ProcessEmployees(List<MemberFinancials> members, UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest,
        AdjustmentReportData adjustmentReportData, CancellationToken cancellationToken)
    {
        var isReRunNeeded = false;
        var fiscalDates = await _calendarService.GetYearStartAndEndAccountingDatesAsync(updateAdjustmentAmountsRequest.ProfitYear, cancellationToken);
        List<EmployeeFinancials> employeeFinancialsList = await _dbContextFactory.UseReadOnlyContext(async ctx =>
        {
            IQueryable<ParticipantTotalVestingBalanceDto> totalVestingBalances =
                _totalService.TotalVestingBalance(ctx, (short)(updateAdjustmentAmountsRequest.ProfitYear - 1), fiscalDates.FiscalEndDate);

            return await ctx.PayProfits
                .Include(pp => pp.Demographic)
                .Include(pp => pp.Demographic!.ContactInfo)
                .Where(pp => pp.ProfitYear == (updateAdjustmentAmountsRequest.ProfitYear - 1))
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
                        PointsEarned = (int)pp.PointsEarned!, // This is supposed to be int in the database.   Database will be updated.
                        EtvaAfterVestingRules = tvb.Etva
                    }
                )
                .OrderBy(ef => ef.EmployeeId)
                .ToListAsync(cancellationToken);
        });

        foreach (EmployeeFinancials empl in employeeFinancialsList)
        {
            // if employee is not participating 
            if (empl.EnrolledId != Enrollment.Constants.NotEnrolled || empl.YearsInPlan != 0)
            {
                var ( memb, isReRun ) = await ProcessEmployee(empl, updateAdjustmentAmountsRequest, adjustmentReportData, cancellationToken);
                members.Add(memb);
                isReRunNeeded |= isReRun;
            }
        }

        return isReRunNeeded;
    }

    private async Task ProcessBeneficiaries(List<MemberFinancials> members, UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest, CancellationToken cancellationToken)
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

            MemberFinancials memb = await ProcessBeneficiary(bene, updateAdjustmentAmountsRequest, cancellationToken);
            members.Add(memb);
        }
    }

    private async Task<(MemberFinancials, bool)> ProcessEmployee(EmployeeFinancials empl, UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest,
        AdjustmentReportData adjustmentReportData, CancellationToken cancellationToken)
    {

        // Gets this years profit sharing transactions, aka Distributions - hardships
        DetailTotals detailTotals = await GetDetailTotals(updateAdjustmentAmountsRequest.ProfitYear, empl.Ssn, cancellationToken);

        // MemberTotals holds newly computed values, not old values
        MemberTotals memberTotals = new();

        memberTotals.ContributionAmount =
            ComputeContribution(empl.PointsEarned, empl.EmployeeId, updateAdjustmentAmountsRequest, adjustmentReportData);
        memberTotals.IncomingForfeitureAmount =
            ComputeForfeitures(empl.PointsEarned, empl.EmployeeId, updateAdjustmentAmountsRequest, adjustmentReportData);

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
            memberTotals.EarnPoints = (int)Math.Round(memberTotals.PointsDollars / 100, MidpointRounding.AwayFromZero);
        }

        ComputeEarnings(memberTotals, null, empl, updateAdjustmentAmountsRequest, adjustmentReportData,
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

        bool rerunNeeded = false;
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
            rerunNeeded = true; 
        }
        // --- End Max Contribution

        empl.Contributions = memberTotals.ContributionAmount;
        empl.IncomeForfeiture = memberTotals.IncomingForfeitureAmount;
        return (memberFinancials, rerunNeeded);
    }


    private async Task<MemberFinancials> ProcessBeneficiary(BeneficiaryFinancials bene, UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest, CancellationToken cancellationToken)
    {
        DetailTotals detailTotals = await GetDetailTotals(updateAdjustmentAmountsRequest.ProfitYear, bene.Ssn, cancellationToken);

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
        AdjustmentReportData adjustmentReportData)
    {
        decimal contributionAmount = Math.Round(updateAdjustmentAmountsRequest.ContributionPercent * PointsEarned, 2,
            MidpointRounding.AwayFromZero);

        if (updateAdjustmentAmountsRequest.BadgeToAdjust > 0 && updateAdjustmentAmountsRequest.BadgeToAdjust == badge)
        {
            adjustmentReportData.ContributionAmountUnadjusted = contributionAmount;
            contributionAmount += updateAdjustmentAmountsRequest.AdjustContributionAmount;
            adjustmentReportData.ContributionAmountAdjusted = contributionAmount;
        }

        return contributionAmount;
    }


    private static decimal ComputeForfeitures(long PointsEarned, long badge, UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest,
        AdjustmentReportData adjustmentReportData)
    {
        decimal incomingForfeitureAmount = Math.Round(updateAdjustmentAmountsRequest.IncomingForfeitPercent * PointsEarned, 2,
            MidpointRounding.AwayFromZero);
        if (updateAdjustmentAmountsRequest.BadgeToAdjust > 0 && updateAdjustmentAmountsRequest.BadgeToAdjust == badge)
        {
            adjustmentReportData.IncomingForfeitureAmountUnadjusted = incomingForfeitureAmount;
            incomingForfeitureAmount += updateAdjustmentAmountsRequest.AdjustIncomingForfeitAmount;
            adjustmentReportData.IncomingForfeitureAmountAdjusted = incomingForfeitureAmount;
        }

        return incomingForfeitureAmount;
    }

    // The fact that this method takes either a bene or an empl and has all this conditional logic is not great.
    private static void ComputeEarnings(MemberTotals memberTotals, BeneficiaryFinancials? bene, EmployeeFinancials? empl,
        UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest, AdjustmentReportData? adjustmentsApplied, decimal ClassActionFundTotal)
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

