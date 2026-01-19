using Demoulas.ProfitSharing.Common.Constants;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.ProfitShareEdit;

internal static class EmployeeProcessorHelper
{
    public static async Task<(List<MemberFinancials>, bool)> ProcessEmployees(IProfitSharingDataContextFactory dbContextFactory, ICalendarService calendarService,
        TotalService totalService, IDemographicReaderService demographicReaderService, ProfitShareUpdateRequest profitShareUpdateRequest,
        AdjustmentsSummaryDto adjustmentsSummaryDto, TimeProvider timeProvider, CancellationToken cancellationToken)
    {
        bool employeeExceededMaxContribution = false;
        short currentYear = (short)timeProvider.GetLocalNow().Year;
        short profitYear = profitShareUpdateRequest.ProfitYear;
        short priorYear = (short)(profitShareUpdateRequest.ProfitYear - 1);

        // We want everything up to the beginning of the profit year year, so we use priorYear in this lookup.
        CalendarResponseDto fiscalDates = await calendarService.GetYearStartAndEndAccountingDatesAsync(priorYear, cancellationToken);
        var employeeFinancialsList = await dbContextFactory.UseReadOnlyContext(async ctx =>
        {
            var frozenDemographicQuery = await demographicReaderService.BuildDemographicQueryAsync(ctx, true);
            var employees = ctx.PayProfits
                .Join(ctx.PayProfits,
                    ppYe => ppYe.DemographicId,
                    ppNow => ppNow.DemographicId,
                    (ppYe, ppNow) => new { ppYE = ppYe, ppNow })
                .Where(x => x.ppYE.ProfitYear == profitYear && x.ppNow.ProfitYear == currentYear)
                .Join(frozenDemographicQuery, pp => pp.ppNow.DemographicId, d => d.Id, (pp, frozenDemographics) => new { ppYE = pp.ppYE, ppNow = pp.ppNow, frozenDemographics })
                .Select(x => new
                {
                    x.frozenDemographics.BadgeNumber,
                    x.ppYE.Demographic!.Ssn,
                    Name = x.ppYE.Demographic.ContactInfo!.FullName,
                    EnrolledId = x.ppYE.VestingScheduleId == 0
                        ? EnrollmentConstants.NotEnrolled
                        : x.ppYE.HasForfeited
                            ? x.ppYE.VestingScheduleId == VestingSchedule.Constants.OldPlan
                                ? EnrollmentConstants.OldVestingPlanHasForfeitureRecords
                                : EnrollmentConstants.NewVestingPlanHasForfeitureRecords
                            : x.ppYE.VestingScheduleId == VestingSchedule.Constants.OldPlan
                                ? EnrollmentConstants.OldVestingPlanHasContributions
                                : EnrollmentConstants.NewVestingPlanHasContributions,
                    x.ppYE.EmployeeTypeId,
                    PointsEarned = (int)(x.ppYE.PointsEarned ?? 0),
                    x.ppYE.ZeroContributionReasonId,
                    x.ppNow.Etva,
                    // We use the ppNow Etva here - For example, in the 2024 profit year, we use the ETVA on the 2025 row,
                    // as that is where the current ETVA is.  The 2024 row is meaningless (or will be) populated with "Last Years" ETVA
                    // when we complete the 2024 YE Run.
                    x.ppYE.Demographic.PayFrequencyId,
                });

            var employeeWithBalances =
            (
                from et in employees
                join balTbl in totalService.TotalVestingBalance(ctx, profitYear, priorYear, fiscalDates.FiscalEndDate) on et.Ssn equals balTbl.Ssn into balTmp
                from bal in balTmp.DefaultIfEmpty()
                join thisYr in TotalService.GetProfitDetailTotalsForASingleYear(ctx, profitYear) on et.Ssn equals thisYr.Ssn into txThsYrEnum
                from txns in txThsYrEnum.DefaultIfEmpty()
                select new EmployeeFinancials
                {
                    BadgeNumber = et.BadgeNumber,
                    Ssn = et.Ssn,
                    Name = et.Name,
                    EnrolledId = et.EnrolledId,
                    YearsInPlan = (bal.YearsInPlan ?? 0),
                    CurrentAmount = (bal.CurrentBalance ?? 0),
                    EmployeeTypeId = et.EmployeeTypeId,
                    PointsEarned = et.PointsEarned,
                    Etva = et.Etva,
                    ZeroContributionReasonId = et.ZeroContributionReasonId,
                    PayFrequencyId = et.PayFrequencyId,

                    // Transactions for this year.
                    DistributionsTotal = txns.DistributionsTotal,
                    ForfeitsTotal = txns.ForfeitsTotal,
                    AllocationsTotal = txns.AllocationsTotal,
                    PaidAllocationsTotal = txns.PaidAllocationsTotal,
                    MilitaryTotal = txns.MilitaryTotal,
                    ClassActionFundTotal = txns.ClassActionFundTotal
                });

            return await employeeWithBalances.ToListAsync(cancellationToken);
        }, cancellationToken);

        List<MemberFinancials> members = new();
        foreach (EmployeeFinancials empl in employeeFinancialsList)
        {
            if (empl.EnrolledId != /*0*/ EnrollmentConstants.NotEnrolled || empl.YearsInPlan != 0 || empl.EmployeeTypeId == /*1*/ EmployeeType.Constants.NewLastYear ||
                empl.HasTransactionAmounts() || empl.ZeroContributionReasonId != /*0*/ ZeroContributionReason.Constants.Normal || empl.CurrentAmount != 0)
            {
                var profitDetailTotals = new ProfitDetailTotals(empl.DistributionsTotal ?? 0, empl.ForfeitsTotal ?? 0,
                    empl.AllocationsTotal ?? 0, empl.PaidAllocationsTotal ?? 0, empl.MilitaryTotal ?? 0, empl.ClassActionFundTotal ?? 0);

                (MemberFinancials memb, bool didEmployeeExceededMaxContribution) = ProcessEmployee(empl, profitDetailTotals, profitShareUpdateRequest, adjustmentsSummaryDto);
                members.Add(memb);
                employeeExceededMaxContribution |= didEmployeeExceededMaxContribution;
            }
        }

        return (members, employeeExceededMaxContribution);
    }

    private static (MemberFinancials, bool) ProcessEmployee(EmployeeFinancials empl, ProfitDetailTotals profitDetailTotals, ProfitShareUpdateRequest profitShareUpdateRequest,
        AdjustmentsSummaryDto adjustmentsSummaryData)
    {
        // MemberTotals holds newly computed values, not old values
        MemberTotals memberTotals = new();

        memberTotals.ContributionAmount =
            ComputeContribution(empl.PointsEarned, empl.BadgeNumber, profitShareUpdateRequest, adjustmentsSummaryData);
        memberTotals.IncomingForfeitureAmount =
            ComputeForfeitures(empl.PointsEarned, empl.BadgeNumber, profitShareUpdateRequest, adjustmentsSummaryData);

        // This "EarningsBalance" is actually the new Current Balance.  Consider changing the name
        // Note that CAF gets added here, but subtracted in the next line.   Odd.
        memberTotals.NewCurrentAmount = profitDetailTotals.AllocationsTotal + profitDetailTotals.ClassActionFundTotal +
                                        (empl.CurrentAmount - profitDetailTotals.ForfeitsTotal -
                                         profitDetailTotals.PaidAllocationsTotal) -
                                        profitDetailTotals.DistributionsTotal;
        memberTotals.NewCurrentAmount -= profitDetailTotals.ClassActionFundTotal;


        if (memberTotals.NewCurrentAmount > 0)
        {
            memberTotals.PointsDollars = Math.Round(
                profitDetailTotals.AllocationsTotal
                + (empl.CurrentAmount - profitDetailTotals.ForfeitsTotal - profitDetailTotals.PaidAllocationsTotal)
                - profitDetailTotals.DistributionsTotal
                , 2, MidpointRounding.AwayFromZero);
            memberTotals.EarnPoints = (int)Math.Round(memberTotals.PointsDollars / 100, 0, MidpointRounding.AwayFromZero);
        }

        ComputeEarningsEmployee(empl, memberTotals, profitShareUpdateRequest, adjustmentsSummaryData, profitDetailTotals.ClassActionFundTotal);

        MemberFinancials memberFinancials = new(empl, profitDetailTotals, memberTotals);

        bool employeeExceededMaxContribution =
            //  WARNING: may modify "memberFinancials"
            IsEmployeeExceedingMaxContribution(profitShareUpdateRequest.MaxAllowedContributions, profitDetailTotals.MilitaryTotal, memberTotals, memberFinancials);

        empl.Contributions = memberTotals.ContributionAmount;
        empl.IncomeForfeiture = memberTotals.IncomingForfeitureAmount;
        return (memberFinancials, employeeExceededMaxContribution);
    }

    private static bool IsEmployeeExceedingMaxContribution(decimal maxAllowedContributions, decimal militaryTotal, MemberTotals memberTotals, MemberFinancials memberFinancials)
    {
        decimal memberTotalContribution = memberTotals.ContributionAmount + militaryTotal + memberFinancials.IncomingForfeitures;
        if (memberTotalContribution <= maxAllowedContributions)
        {
            return false;
        }

        decimal overContribution = memberTotalContribution - maxAllowedContributions;
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
        return true;
    }

    private static decimal ComputeContribution(long pointsEarned, long badge, ProfitShareUpdateRequest profitShareUpdateRequest,
        AdjustmentsSummaryDto adjustmentsSummaryData)
    {
        decimal contributionAmount = Math.Round(profitShareUpdateRequest.ContributionPercent * pointsEarned, 2,
            MidpointRounding.AwayFromZero);

        if (profitShareUpdateRequest.BadgeToAdjust > 0 && profitShareUpdateRequest.BadgeToAdjust == badge)
        {
            adjustmentsSummaryData.ContributionAmountUnadjusted = contributionAmount;
            contributionAmount += profitShareUpdateRequest.AdjustContributionAmount;
            adjustmentsSummaryData.ContributionAmountAdjusted = contributionAmount;
        }

        return contributionAmount;
    }


    private static decimal ComputeForfeitures(long pointsEarned, long badge, ProfitShareUpdateRequest profitShareUpdateRequest,
        AdjustmentsSummaryDto adjustmentsSummaryData)
    {
        decimal incomingForfeitureAmount = Math.Round(profitShareUpdateRequest.IncomingForfeitPercent * pointsEarned, 2, MidpointRounding.AwayFromZero);
        if (profitShareUpdateRequest.BadgeToAdjust > 0 && profitShareUpdateRequest.BadgeToAdjust == badge)
        {
            adjustmentsSummaryData.IncomingForfeitureAmountUnadjusted = incomingForfeitureAmount;
            incomingForfeitureAmount += profitShareUpdateRequest.AdjustIncomingForfeitAmount;
            adjustmentsSummaryData.IncomingForfeitureAmountAdjusted = incomingForfeitureAmount;
        }

        return incomingForfeitureAmount;
    }


    private static void ComputeEarningsEmployee(EmployeeFinancials empl, MemberTotals memberTotals, ProfitShareUpdateRequest profitShareUpdateRequest,
        AdjustmentsSummaryDto adjustmentsApplied, decimal classActionFundTotal)
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
            adjustmentsApplied.EarningsAmountUnadjusted = memberTotals.EarningsAmount;
            memberTotals.EarningsAmount += profitShareUpdateRequest.AdjustEarningsAmount;
            adjustmentsApplied.EarningsAmountAdjusted = memberTotals.EarningsAmount;
        }

        memberTotals.SecondaryEarningsAmount =
            Math.Round(profitShareUpdateRequest.SecondaryEarningsPercent * memberTotals.EarnPoints, 2,
                MidpointRounding.AwayFromZero);
        if (profitShareUpdateRequest.BadgeToAdjust2 > 0 && profitShareUpdateRequest.BadgeToAdjust2 == (empl?.BadgeNumber ?? 0))
        {
            adjustmentsApplied.SecondaryEarningsAmountUnadjusted = memberTotals.SecondaryEarningsAmount;
            memberTotals.SecondaryEarningsAmount += profitShareUpdateRequest.AdjustEarningsSecondaryAmount;
            adjustmentsApplied.SecondaryEarningsAmountAdjusted = memberTotals.SecondaryEarningsAmount;
        }

        decimal workingEtva = 0;
        // When the CAF (Class Action Fund) is present and the member is under 6 in YIP (Years in Service/Plan),
        // Need to subtract CAF out of PY-PS-ETVA (ETVA) for people not fully vested because we can't give earnings for 2021 on class action funds -
        // they were added in 2021.  CAF was added to PY-PS-ETVA (ETVA) for PY-PS-YEARS < 6.
        if (empl!.Etva > 0)
        {
            // This check for 6 years is only used here, so it is intentionally not pulled out.
            // It is presumed that this 6 is specific to the 2021 adjustment.  It is assumed that the
            // plan enrollment type is specifically not consulted (i.e. No OLD vs NEW plan)
            if (empl.YearsInPlan < 6)
            {
                workingEtva = empl.Etva - classActionFundTotal;
            }
            else
            {
                workingEtva = empl.Etva;
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
        // The Cobol truncates (and not rounds) to 6 places, so we do the same here.
        etvaScaled = Math.Truncate(etvaScaled * 1_000_000) / 1_000_000;

        // Sets Earn and ETVA amounts
        empl!.Earnings = memberTotals.EarningsAmount;
        empl.EarningsOnEtva = Math.Round(memberTotals.EarningsAmount * etvaScaled, 2, MidpointRounding.AwayFromZero);

        empl.SecondaryEarnings = memberTotals.SecondaryEarningsAmount;
        empl.EarningsOnSecondaryEtva = Math.Round(memberTotals.SecondaryEarningsAmount * etvaScaled, 2, MidpointRounding.AwayFromZero);
    }
}
