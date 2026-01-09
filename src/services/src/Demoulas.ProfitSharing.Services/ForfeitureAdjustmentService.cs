using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

public class ForfeitureAdjustmentService : IForfeitureAdjustmentService
{
    private readonly IProfitSharingDataContextFactory _dbContextFactory;
    private readonly TotalService _totalService;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TimeProvider _timeProvider;

    public ForfeitureAdjustmentService(IProfitSharingDataContextFactory dbContextFactory,
        TotalService totalService,
        IDemographicReaderService demographicReaderService,
        TimeProvider timeProvider)
    {
        _dbContextFactory = dbContextFactory;
        _totalService = totalService;
        _demographicReaderService = demographicReaderService;
        _timeProvider = timeProvider;
    }

    // Invoked by the 008-12 Forfeit Adjustment screen - the user wants to make a change, we figure out which (forfeit or unforfeit or neither) is appropriate.
    public Task<Result<SuggestedForfeitureAdjustmentResponse>> GetSuggestedForfeitureAmount(SuggestedForfeitureAdjustmentRequest req, CancellationToken cancellationToken = default)
    {
        return _dbContextFactory.UseReadOnlyContext(async context =>
        {
            Employee? empl = await FetchEmployee(context, req.Badge, req.Ssn, cancellationToken);

            if (empl == null)
            {
                return Result<SuggestedForfeitureAdjustmentResponse>.Failure(Error.EmployeeNotFound);
            }

            // Get the most recent PROFIT_CODE = 2 (forfeiture) transaction for this employee
            var lastForfeitureTransaction = context.ProfitDetails
                .Where(pd => pd.Ssn == empl.Demographic.Ssn && pd.ProfitCodeId == 2 && pd.CommentTypeId != CommentType.Constants.ForfeitClassAction)
                .OrderByDescending(pd => pd.ProfitYear)
                .ThenByDescending(pd => pd.CreatedAtUtc)
                .FirstOrDefault();

            // PATH 1: Last transaction was a FORFEIT (positive amount)
            // → Suggest UNFORFEIT (negative amount to reverse it)
            if (lastForfeitureTransaction != null && lastForfeitureTransaction.Forfeiture > 0)
            {
                return Suggested(empl, -lastForfeitureTransaction.Forfeiture); // Negative = unforfeit
            }

            // This service is always live, so our reference time is today.
            var asOfDate = _timeProvider.GetLocalDateOnly();
            var profitYear = (short) asOfDate.Year;

            // PATH 2: No forfeiture history OR last transaction was UNFORFEIT (negative amount)
            // → Calculate how much SHOULD be forfeited based on vesting
            var totalVestingBalance = await _totalService.TotalVestingBalance(context, profitYear, asOfDate)
                .Where(vb => vb.Ssn == empl.Demographic.Ssn).SingleOrDefaultAsync(cancellationToken);

            // If no vesting balance found or fully vested, nothing to forfeit
            if (totalVestingBalance == null || totalVestingBalance.VestingPercent == 1m || totalVestingBalance.CurrentBalance <= 0m)
            {
                return Suggested(empl, 0m);
            }

            // Calculate unvested amount that should be forfeited
            var unvestedAmount = (totalVestingBalance.CurrentBalance ?? 0m) - (totalVestingBalance.VestedBalance ?? 0m);

            if (empl.PayProfit?.Etva > 0)
            {
                decimal conditional = (totalVestingBalance?.CurrentBalance ?? 0) - empl.PayProfit.Etva;
                unvestedAmount = conditional * (1 - totalVestingBalance?.VestingPercent ?? 1);
                if (unvestedAmount < 0m)
                {
                    return Suggested(empl, 0);
                }
            }

            return Suggested(empl, unvestedAmount);
        }, cancellationToken);
    }

    private static Result<SuggestedForfeitureAdjustmentResponse> Suggested(Employee empl, decimal suggestedAmount)
    {
        return Result<SuggestedForfeitureAdjustmentResponse>.Success(new SuggestedForfeitureAdjustmentResponse
        {
            SuggestedForfeitAmount = Math.Round(suggestedAmount, 2, MidpointRounding.AwayFromZero),
            DemographicId = empl.Demographic.Id,
            BadgeNumber = empl.Demographic.BadgeNumber
        });
    }
    

    private async Task<Employee?> FetchEmployee(IProfitSharingDbContext ctx, int? badgeMaybe, int? ssnMaybe, CancellationToken ct)
    {
        var profitYear = _timeProvider.GetLocalYear();
        var demographics = await _demographicReaderService.BuildDemographicQuery(ctx, false);

        return await demographics
            .Where(d => ssnMaybe.HasValue && ssnMaybe.Value == d.Ssn || badgeMaybe.HasValue && badgeMaybe.Value == d.BadgeNumber)
            .Select(d => new Employee
            {
                Demographic = d,
                PayProfit = d.PayProfits.FirstOrDefault(pp => pp.ProfitYear == profitYear)!
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Result<bool>> UpdateForfeitureAdjustmentAsync(ForfeitureAdjustmentUpdateRequest req, CancellationToken cancellationToken = default)
    {
        if (req.ForfeitureAmount == 0)
        {
            return Result<bool>.Failure(Error.ForfeitureAmountZero);
        }

        // We can probably only do the wall clock year or the frozen year.
        if (req.ProfitYear <= 0)
        {
            return Result<bool>.Failure(Error.InvalidProfitYear);
        }

        return await _dbContextFactory.UseWritableContext(async context =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(context, false);
            var employeeData = await demographics
                .Where(d => d.BadgeNumber == req.BadgeNumber)
                .Select(d => new
                {
                    d.Id,
                    d.Ssn,
                    d.BadgeNumber,
                    d.StoreNumber,
                    HasPayProfit = context.PayProfits.Any(pp => pp.DemographicId == d.Id && pp.ProfitYear == req.ProfitYear)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (employeeData == null)
            {
                return Result<bool>.Failure(Error.EmployeeNotFound);
            }

            if (!employeeData.HasPayProfit)
            {
                return Result<bool>.Failure(Error.NoPayProfitDataForYear);
            }

            // Determine if this is a forfeit or un-forfeit operation
            bool isForfeit = req.ForfeitureAmount > 0;

            if (req.OffsettingProfitDetailId.HasValue)
            {
                var offsettingProfitDetail = await context.ProfitDetails
                    .FirstOrDefaultAsync(pd => pd.Id == req.OffsettingProfitDetailId.Value, cancellationToken);
                if (offsettingProfitDetail == null)
                {
                    return Result<bool>.Failure(Error.ProfitDetailNotFound);
                }

                if (offsettingProfitDetail?.CommentTypeId == CommentType.Constants.ForfeitClassAction && !isForfeit)
                {
                    return Result<bool>.Failure(Error.ClassActionForfeitureCannotBeReversed);
                }

                // Check if this profit detail has already been reversed (double-reversal protection)
                var alreadyReversed = await context.ProfitDetails
                    .AnyAsync(pd => pd.ReversedFromProfitDetailId == req.OffsettingProfitDetailId.Value, cancellationToken);
                if (alreadyReversed)
                {
                    return Result<bool>.Failure(Error.ProfitDetailAlreadyReversed);
                }
            }

            // Get vesting balance from the total service
            var vestingBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(
                SearchBy.Ssn,
                employeeData.Ssn,
                req.ProfitYear,
                cancellationToken);

            // If no vesting balance found, return failure
            if (vestingBalance == null)
            {
                return Result<bool>.Failure(Error.VestingBalanceNotFound);
            }

            // From docs: "From the screen in figure 2, if you enter the value in #17 to box #12 and hit enter, you will create a PROFIT_DETAIL record.
            // When the value is negative the record has UN-FORFEIT in the PROFIT_CMNT field and when the value is positive the PROFIT_CMNT field is FORFEIT."
            CommentType commentType = (isForfeit, req.ClassAction) switch
            {
                (true, false) => CommentType.Constants.Forfeit,
                (true, true) => CommentType.Constants.ForfeitClassAction,
                (false, _) => CommentType.Constants.Unforfeit,
            };
            string remarkText = commentType.Name.ToUpper();

            // Create a new PROFIT_DETAIL record
            var profitDetail = new ProfitDetail
            {
                Ssn = employeeData.Ssn,
                ProfitYear = req.ProfitYear,
                ProfitYearIteration = 0,
                ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id, // Code 2 for forfeitures
                Remark = remarkText,
                Forfeiture = req.ForfeitureAmount,
                MonthToDate = _timeProvider.GetLocalMonthAsByte(),
                YearToDate = _timeProvider.GetLocalYearAsShort(),
                CreatedAtUtc = DateTimeOffset.UtcNow,
                ModifiedAtUtc = DateTimeOffset.UtcNow,
                CommentTypeId = commentType.Id,
                ReversedFromProfitDetailId = req.OffsettingProfitDetailId, // Track which record was reversed
            };

            context.ProfitDetails.Add(profitDetail);

            var payProfit = await context.PayProfits.Include(p => p.Demographic)
                .FirstOrDefaultAsync(pp => pp.DemographicId == employeeData.Id && pp.ProfitYear == req.ProfitYear, cancellationToken);

            if (payProfit != null)
            {
                // Get Calculated ETVA amount
                var profitCodeTotals = await _totalService.GetTotalComputedEtva(context, req.ProfitYear).Where(x => x.Ssn == payProfit.Demographic!.Ssn)
                    .FirstOrDefaultAsync(cancellationToken);

                // Default to zero if no totals found
                var calculatedEtva = profitCodeTotals != default ? profitCodeTotals.TotalAmount : 0m;

                // Prevent negative ETVA
                if (calculatedEtva < 0)
                {
                    calculatedEtva = 0m;
                }

                // From docs: "When this profit detail record is created, the PAYPROFIT record will also be updated.
                // If the record is FORFEIT then the field PY_PS_ETVA will be updated to zero and the PY_PS_ENROLLED value will added to by 2.
                // So 1 goes to 3 and 2 goes to 4. When the UN-FORFEIT gets created the opposite happens to PAYPROFIT.
                // The PY_PS_ETVA gets calculated then written and PY_PS_ENROLLED gets subtracted by two. So 3 becomes 1 and 4 becomes 2."
                if (isForfeit)
                {
                    int wallClockYear = _timeProvider.GetLocalYear();
                    if (req.ProfitYear <= wallClockYear - 2)
                    {
                        return Result<bool>.Failure(Error.Unexpected(
                            $"Cannot update profit year {req.ProfitYear}. Only current year ({wallClockYear}) and previous year ({wallClockYear - 1}) are allowed."));
                    }

                    // Determine live year set - normally just current year, but includes previous year for special cases (YE)
                    var liveYearSet = req.ProfitYear == wallClockYear - 1
                        ? new[] { wallClockYear - 1, wallClockYear }
                        : new[] { wallClockYear };

                    // For forfeit: Set PY_PS_ETVA to 0 and increment enrollment by 2
                    byte newEnrollmentId = Enrollment.Constants.NotEnrolled;
                    if (payProfit.EnrollmentId == /*2*/ Enrollment.Constants.NewVestingPlanHasContributions)
                    {
                        newEnrollmentId = /*4*/ Enrollment.Constants.NewVestingPlanHasForfeitureRecords;
                    }
                    else if (payProfit.EnrollmentId == /*1*/ Enrollment.Constants.OldVestingPlanHasContributions)
                    {
                        newEnrollmentId = /*3*/ Enrollment.Constants.OldVestingPlanHasForfeitureRecords;
                    }

                    await context.PayProfits
                        .Where(pp => pp.DemographicId == employeeData.Id && liveYearSet.Contains(pp.ProfitYear))
                        .ExecuteUpdateAsync(p => p
                                .SetProperty(pp => pp.EnrollmentId, newEnrollmentId)
                                .SetProperty(pp => pp.Etva, calculatedEtva) // Set recalculated ETVA
                                .SetProperty(pp => pp.ModifiedAtUtc, DateTimeOffset.UtcNow),
                            cancellationToken);
                }
                else
                {
                    // For un-forfeit: Recalculate PY_PS_ETVA and decrement enrollment by 2
                    byte newEnrollmentId;

                    // Enrollment ID math based on known constants to prevent magic numbers
                    if (payProfit.EnrollmentId == Enrollment.Constants.NewVestingPlanHasForfeitureRecords)
                    {
                        newEnrollmentId = Enrollment.Constants.NewVestingPlanHasContributions;
                    }
                    else if (payProfit.EnrollmentId == Enrollment.Constants.OldVestingPlanHasForfeitureRecords)
                    {
                        newEnrollmentId = Enrollment.Constants.OldVestingPlanHasContributions;
                    }
                    else
                    {
                        // Keep same if not a known constant with a -/+2 option.
                        newEnrollmentId = payProfit.EnrollmentId;
                    }

                    await context.PayProfits
                        .Where(pp => pp.DemographicId == employeeData.Id && pp.ProfitYear == req.ProfitYear)
                        .ExecuteUpdateAsync(p => p
                                .SetProperty(pp => pp.EnrollmentId, newEnrollmentId)
                                .SetProperty(pp => pp.Etva, calculatedEtva) // Set recalculated ETVA
                                .SetProperty(pp => pp.ModifiedAtUtc, DateTimeOffset.UtcNow),
                            cancellationToken);
                }
            }

            await context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }, cancellationToken);
    }

    public async Task<Result<bool>> UpdateForfeitureAdjustmentBulkAsync(List<ForfeitureAdjustmentUpdateRequest> requests, CancellationToken cancellationToken = default)
    {
        foreach (var req in requests)
        {
            var result = await UpdateForfeitureAdjustmentAsync(req, cancellationToken);
            if (result.IsError && result.Error != null)
            {
                return Result<bool>.Failure(result.Error);
            }
        }

        return Result<bool>.Success(true);
    }
}
