using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

public class ForfeitureAdjustmentService : IForfeitureAdjustmentService
{
    private readonly IProfitSharingDataContextFactory _dbContextFactory;
    private readonly TotalService _totalService;
    private readonly IFrozenService _frozenService;
    private readonly IDemographicReaderService _demographicReaderService;

    public ForfeitureAdjustmentService(IProfitSharingDataContextFactory dbContextFactory,
        TotalService totalService, IEmbeddedSqlService embeddedSqlService,
        IFrozenService frozenService,
        IDemographicReaderService demographicReaderService)
    {
        _dbContextFactory = dbContextFactory;
        _totalService = totalService;
        _frozenService = frozenService;
        _demographicReaderService = demographicReaderService;
    }
    
    public Task<SuggestedForfeitureAdjustmentResponse> GetSuggestedForfeitureAmount(SuggestedForfeitureAdjustmentRequest req, CancellationToken cancellationToken = default)
    {
        return _dbContextFactory.UseReadOnlyContext(async context =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(context);
            var frozenStateResponse = await _frozenService.GetActiveFrozenDemographic(cancellationToken);
            short profitYear = frozenStateResponse.ProfitYear;

            var demographic = await demographics
                .Where(d => req.Ssn.HasValue && req.Ssn.Value == d.Ssn || req.Badge.HasValue && req.Badge.Value == d.BadgeNumber)
                .FirstOrDefaultAsync(cancellationToken);

            if (demographic == null)
            {
                throw new ArgumentException("Employee not found.");
            }
            
            var totalVestingBalance = await _totalService.TotalVestingBalance(context, profitYear, DateTime.Now.ToDateOnly())
                .Where(vb => vb.Ssn == demographic.Ssn).SingleAsync();

            return
                // BOBH This is failing to account for the ETVA, correct?
                new SuggestedForfeitureAdjustmentResponse
                {
                    SuggestedForfeitAmount = totalVestingBalance.CurrentBalance ?? 0m - totalVestingBalance.VestedBalance ?? 0m,
                    DemographicId = demographic.Id,
                    BadgeNumber = demographic.BadgeNumber
                };
            
        });
    }
    
    public async Task UpdateForfeitureAdjustmentAsync(ForfeitureAdjustmentUpdateRequest req, CancellationToken cancellationToken = default)
    {
        if (req.ForfeitureAmount == 0)
        {
            throw new ArgumentException("Forfeiture amount cannot be zero");
        }

        // We can probably only do the wall clock year or the frozen year.
        if (req.ProfitYear <= 0)
        {
            throw new ArgumentException("Profit year must be provided");
        }
        
        await _dbContextFactory.UseWritableContext(async context =>
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
                throw new ArgumentException($"Employee with badge number {req.BadgeNumber} not found");
            }

            if (!employeeData.HasPayProfit)
            {
                throw new ArgumentException($"No profit sharing data found for employee with badge number {req.BadgeNumber} for year {req.ProfitYear}");
            }

            // Determine if this is a forfeit or un-forfeit operation
            bool isForfeit = req.ForfeitureAmount > 0;

            if (req.OffsettingProfitDetailId.HasValue)
            {
                var offsettingProfitDetail = await context.ProfitDetails
                    .FirstOrDefaultAsync(pd => pd.Id == req.OffsettingProfitDetailId.Value, cancellationToken);
                if (offsettingProfitDetail == null)
                {
                    throw new InvalidOperationException($"Offsetting profit detail with ID {req.OffsettingProfitDetailId.Value} not found");
                }

                if (offsettingProfitDetail?.CommentTypeId == CommentType.Constants.ForfeitClassAction && !isForfeit)
                {
                    throw new InvalidOperationException(
                        $"Offsetting profit detail with ID {req.OffsettingProfitDetailId.Value} is a class action forfeiture and cannot be unforfeited.");
                }
            }

            // Get vesting balance from the total service
            var vestingBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(
                SearchBy.Ssn,
                employeeData.Ssn,
                (short)req.ProfitYear,
                cancellationToken);

            // If no vesting balance found, throw an exception
            if (vestingBalance == null)
            {
                throw new ArgumentException($"No vesting balance data found for employee with badge number {req.BadgeNumber}");
            }

            // From docs: "From the screen in figure 2, if you enter the value in #17 to box #12 and hit enter, you will create a PROFIT_DETAIL record.
            // When the value is negative the record has UN-FORFEIT in the PROFIT_CMNT field and when the value is positive the PROFIT_CMNT field is FORFEIT."
            CommentType commentType = (isForfeit, req.ClassAction) switch
            {
                (true, false) => CommentType.Constants.Forfeit,
                (true, true) => CommentType.Constants.ForfeitClassAction,
                (false, _) => CommentType.Constants.UnForfeit,
            };
            string remarkText = commentType.Name.ToUpper();    

            // Create a new PROFIT_DETAIL record
            var profitDetail = new ProfitDetail
            {
                Ssn = employeeData.Ssn,
                ProfitYear = (short)req.ProfitYear,
                ProfitYearIteration = 0,
                ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id, // Code 2 for forfeitures
                Remark = remarkText,
                Forfeiture = req.ForfeitureAmount,
                MonthToDate = (byte)DateTime.Now.Month,
                YearToDate = (short)DateTime.Now.Year,
                CreatedAtUtc = DateTimeOffset.UtcNow,
                ModifiedAtUtc = DateTimeOffset.UtcNow,
                CommentTypeId = commentType.Id,
                
            };

            context.ProfitDetails.Add(profitDetail);

            var payProfit = await context.PayProfits
                .FirstOrDefaultAsync(pp => pp.DemographicId == employeeData.Id && pp.ProfitYear == req.ProfitYear, cancellationToken);

            if (payProfit != null)
            {
                // From docs: "When this profit detail record is created, the PAYPROFIT record will also be updated.
                // If the record is FORFEIT then the field PY_PS_ETVA will be updated to zero and the PY_PS_ENROLLED value will added to by 2.
                // So 1 goes to 3 and 2 goes to 4. When the UN-FORFEIT gets created the opposite happens to PAYPROFIT.
                // The PY_PS_ETVA gets calculated then written and PY_PS_ENROLLED gets subtracted by two. So 3 becomes 1 and 4 becomes 2."
                if (isForfeit)
                {
                    int wallClockYear = DateTime.Now.Year;
                    if (req.ProfitYear <= wallClockYear - 2)
                    {
                        throw new ArgumentException(
                            $"Cannot update profit year {req.ProfitYear}. Only current year ({wallClockYear}) and previous year ({wallClockYear - 1}) are allowed.");
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
                                .SetProperty(pp => pp.Etva, 0)  // TBD If you have Allocation Money, then EVTA moves to that value not zero
                                .SetProperty(pp => pp.ModifiedAtUtc, DateTimeOffset.UtcNow),
                            cancellationToken);
                }
                else
                {
                    // For un-forfeit: Recalculate PY_PS_ETVA and decrement enrollment by 2
                    // @Russ/Phil - any insights to recalculate ETVA?
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
                                .SetProperty(pp => pp.ModifiedAtUtc, DateTimeOffset.UtcNow),
                            cancellationToken);
                }
            }

            await context.SaveChangesAsync(cancellationToken);

        }, cancellationToken);
    }

    public async Task UpdateForfeitureAdjustmentBulkAsync(List<ForfeitureAdjustmentUpdateRequest> requests, CancellationToken cancellationToken = default)
    {
        await _dbContextFactory.UseWritableContext(async context =>
        {
            foreach (var req in requests)
            {
                    await UpdateForfeitureAdjustmentAsync(req, cancellationToken);
            }
            await context.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }
}
