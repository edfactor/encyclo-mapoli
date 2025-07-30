using System.Data.SqlTypes;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Services;
public class ForfeitureAdjustmentService : IForfeitureAdjustmentService
{
    private readonly IProfitSharingDataContextFactory _dbContextFactory;
    private readonly ITotalService _totalService;
    private readonly IEmbeddedSqlService _embeddedSqlService;
    private readonly IDemographicReaderService _demographicReaderService;

    public ForfeitureAdjustmentService(IProfitSharingDataContextFactory dbContextFactory, 
        ITotalService totalService, IEmbeddedSqlService embeddedSqlService,
        IDemographicReaderService demographicReaderService)
    {
        _dbContextFactory = dbContextFactory;
        _totalService = totalService;
        _embeddedSqlService = embeddedSqlService;
        _demographicReaderService = demographicReaderService;
    }

    private sealed class EmployeeInfo
    {
        public required int Id { get; init; }
        public required int Ssn { get; init; }
        public required int BadgeNumber { get; init; }
    }

    public Task<ForfeitureAdjustmentReportResponse> GetForfeitureAdjustmentReportAsync(ForfeitureAdjustmentRequest req, CancellationToken cancellationToken = default)
    {
        // Default to return if any need to short circuit
        var response = new ForfeitureAdjustmentReportResponse
        {
            ReportName = ForfeitureAdjustmentReportResponse.REPORT_NAME,
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = ReferenceData.DsmMinValue,
            EndDate = DateTimeOffset.UtcNow.ToDateOnly(),
            TotatNetBalance = 0,
            TotatNetVested = 0,
            Response = new PaginatedResponseDto<ForfeitureAdjustmentReportDetail>
            {
                Results = new List<ForfeitureAdjustmentReportDetail>(),
                Total = 0
            }
        };

        return _dbContextFactory.UseReadOnlyContext(async context =>
        {
            var employeeData = await FindEmployeeAsync(context, req, cancellationToken);

            if (employeeData == null)
            {
                return response;
            }

            var vestingBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(
                Common.Contracts.Request.SearchBy.Ssn,
                employeeData.Ssn,
                req.ProfitYear,
                cancellationToken);

            if (vestingBalance == null)
            {
                return response;
            }

            // Using current balance from the vesting balance - docs say should be PAYPROFT.PY_PS.AMT but not seeing that in the EF mapping.
            // @Russ/Phil - any insights?

            // CurrentBalance comes from TotalsService.GetTotalBalanceSet. Starting balance uses the same method, but with profitYear - 1.
            decimal startingBalance = await _embeddedSqlService.GetTotalBalanceAlt(context, req.ProfitYear)
                .Where(x => x.Ssn == employeeData.Ssn)
                .Select(x => x.Total)
                .FirstOrDefaultAsync(cancellationToken) ?? 0;

            // From doc: Forfeiture Amount - when PROFIT_DETAIL.PROFIT_CODE = 2
            // then accumulate PROFIT_FORT values then multiply by -1
            var forfeitureAmount = await context.ProfitDetails
                .Where(pd =>
                    pd.Ssn == employeeData.Ssn &&
                    pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id) // Code 2 for forfeitures
                .SumAsync(pd => pd.Forfeiture, cancellationToken) * -1; // Multiply by -1 to get positive value

            // From doc: See current balance in calculation document.  That value minus starting balance
            // Gonna need help since PAYPROFT.PY_PS.AMT is not in the EF mapping and starting balance is based on current balance
            decimal netBalance = startingBalance - forfeitureAmount;

            // From doc: See current vested balance in calculation document.
            decimal netVested = vestingBalance.VestedBalance;

            var detail = new ForfeitureAdjustmentReportDetail
            {
                DemographicId = employeeData.Id,
                BadgeNumber = employeeData.BadgeNumber,
                StartingBalance = startingBalance,
                ForfeitureAmount = forfeitureAmount,
                NetBalance = netBalance,
                NetVested = netVested
            };

            // Set response properties - total balance and total vested are same as net values?
            // From docs:
            // 15.	Same as #13.   The calculation to change is obsolete.
            // 16.	Same as #14.   The calculation to change is obsolete.
            response.TotatNetBalance = (int)Math.Round(detail.NetBalance, MidpointRounding.AwayFromZero);
            response.TotatNetVested = (int)Math.Round(detail.NetVested, MidpointRounding.AwayFromZero);

            response.Response = new PaginatedResponseDto<ForfeitureAdjustmentReportDetail>
            {
                Results = new List<ForfeitureAdjustmentReportDetail> { detail },
                Total = 1 // will there ever be more than one? legacy screenshots show 1 datapoint, but mulitple rows for entry...
            };

            return response;
        });
    }
    
    private async Task<EmployeeInfo?> FindEmployeeAsync(ProfitSharingReadOnlyDbContext context,
        ForfeitureAdjustmentRequest req, CancellationToken cancellationToken)
    {
        var demographics =  await _demographicReaderService.BuildDemographicQuery(context);

        // If SSN is provided, use that for lookup
        if (req.SSN > 0)
        {
            return await demographics
                .Where(d => d.Ssn == req.SSN.Value)
                .Where(d => context.PayProfits.Any(pp => pp.DemographicId == d.Id && pp.ProfitYear == req.ProfitYear))
                .Select(d => new EmployeeInfo
                {
                    Id = d.Id,
                    Ssn = d.Ssn,
                    BadgeNumber = d.BadgeNumber
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
        // Otherwise, if Badge is provided, use that for lookup
        if (req.Badge > 0)
        {
            return await demographics
                .Where(d => d.BadgeNumber == req.Badge.Value)
                .Where(d => context.PayProfits.Any(pp => pp.DemographicId == d.Id && pp.ProfitYear == req.ProfitYear))
                .Select(d => new EmployeeInfo
                {
                    Id = d.Id,
                    Ssn = d.Ssn,
                    BadgeNumber = d.BadgeNumber
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        return null;
    }

    public async Task<ForfeitureAdjustmentReportDetail> UpdateForfeitureAdjustmentAsync(ForfeitureAdjustmentUpdateRequest req, CancellationToken cancellationToken = default)
    {
        if (req.ForfeitureAmount == 0)
        {
            throw new ArgumentException("Forfeiture amount cannot be zero");
        }

        if (req.ProfitYear <= 0)
        {
            throw new ArgumentException("Profit year must be provided");
        }

        ForfeitureAdjustmentReportDetail? result = null;

        await _dbContextFactory.UseWritableContext(async context =>
        {
            var demographics= await _demographicReaderService.BuildDemographicQuery(context, false);
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
                    throw new InvalidOperationException($"Offsetting profit detail with ID {req.OffsettingProfitDetailId.Value} is a class action forfeiture and cannot be unforfeited.");
                }
            }

            // Get vesting balance from the total service
            var vestingBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(
                Common.Contracts.Request.SearchBy.Ssn,
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
            string remarkText = isForfeit ? CommentType.Constants.Forfeit.Name.ToUpper() : CommentType.Constants.UnForfeit.Name.ToUpper();

            // Create a new PROFIT_DETAIL record
            var profitDetail = new ProfitDetail
            {
                Ssn = employeeData.Ssn,
                ProfitYear = (short)req.ProfitYear,
                ProfitYearIteration = 0,
                ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id, // Code 2 for forfeitures
                Remark = remarkText,
                Forfeiture = Math.Abs(req.ForfeitureAmount) * (isForfeit ? -1 : 1), // Negative for forfeit, positive for un-forfeit, we'll need to double check this logic
                MonthToDate = (byte)DateTime.Now.Month,
                YearToDate = (short)DateTime.Now.Year,
                TransactionDate = DateTimeOffset.Now,
                ModifiedAtUtc = DateTimeOffset.UtcNow
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
                        throw new ArgumentException($"Cannot update profit year {req.ProfitYear}. Only current year ({wallClockYear}) and previous year ({wallClockYear - 1}) are allowed.");
                    }

                    // Determine live year set - normally just current year, but includes previous year for special cases (YE)
                    var liveYearSet = req.ProfitYear == wallClockYear - 1 
                        ? new[] { wallClockYear - 1, wallClockYear }
                        : new[] { wallClockYear };

                    // For forfeit: Set PY_PS_ETVA to 0 and increment enrollment by 2
                    byte newEnrollmentId;
                    if (payProfit.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions)
                    {
                        newEnrollmentId = Enrollment.Constants.NewVestingPlanHasForfeitureRecords;
                    }
                    else if (payProfit.EnrollmentId == Enrollment.Constants.OldVestingPlanHasContributions)
                    {
                        newEnrollmentId = Enrollment.Constants.OldVestingPlanHasForfeitureRecords;
                    }
                    else
                    {
                        newEnrollmentId = Enrollment.Constants.NotEnrolled;
                    }

                    await context.PayProfits
                        .Where(pp => pp.DemographicId == employeeData.Id && liveYearSet.Contains(pp.ProfitYear))
                        .ExecuteUpdateAsync(p => p
                            .SetProperty(pp => pp.EnrollmentId, newEnrollmentId)
                            .SetProperty(pp => pp.Etva, 0)
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

            // Return the updated entity to the frontend to update grid
            decimal startingBalance = vestingBalance.CurrentBalance;
            decimal forfeitureAmount = req.ForfeitureAmount;
            decimal netBalance = startingBalance - forfeitureAmount;
            decimal netVested = vestingBalance.VestedBalance;

            if (forfeitureAmount > 0)
            {
                // If there are forfeitures, adjust the vested balance accordingly
                netVested = Math.Max(0, vestingBalance.VestedBalance - forfeitureAmount);
            }
            else
            {
                // For un-forfeit, we may need to adjust vested balance
                // This logic would depend on business rules
            }

            // Create the response
            result = new ForfeitureAdjustmentReportDetail
            {
                DemographicId = employeeData.Id,
                BadgeNumber = req.BadgeNumber,
                StartingBalance = startingBalance,
                ForfeitureAmount = forfeitureAmount,
                NetBalance = netBalance,
                NetVested = netVested
            };
        }, cancellationToken);

        return result ?? new ForfeitureAdjustmentReportDetail
        {
            DemographicId = 0,
            BadgeNumber = 0,
            StartingBalance = 0,
            ForfeitureAmount = 0,
            NetBalance = 0,
            NetVested = 0
        };
    }

    public async Task<List<ForfeitureAdjustmentReportDetail>> UpdateForfeitureAdjustmentBulkAsync(List<ForfeitureAdjustmentUpdateRequest> requests, CancellationToken cancellationToken = default)
    {
        var results = new List<ForfeitureAdjustmentReportDetail>();

        await _dbContextFactory.UseWritableContext(async context =>
        {
            foreach (var req in requests)
            {
                if (req.ForfeitureAmount == 0)
                {
                    throw new ArgumentException($"Forfeiture amount cannot be zero for badge {req.BadgeNumber}");
                }

                if (req.ProfitYear <= 0)
                {
                    throw new ArgumentException($"Profit year must be provided for badge {req.BadgeNumber}");
                }

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
                        throw new InvalidOperationException($"Offsetting profit detail with ID {req.OffsettingProfitDetailId.Value} is a class action forfeiture and cannot be unforfeited.");
                    }
                }

                // Get vesting balance from the total service
                var vestingBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(
                    Common.Contracts.Request.SearchBy.Ssn,
                    employeeData.Ssn,
                    (short)req.ProfitYear,
                    cancellationToken);

                // If no vesting balance found, throw an exception
                if (vestingBalance == null)
                {
                    throw new ArgumentException($"No vesting balance data found for employee with badge number {req.BadgeNumber}");
                }

                string remarkText = isForfeit ? CommentType.Constants.Forfeit.Name.ToUpper() : CommentType.Constants.UnForfeit.Name.ToUpper();

                // Create a new PROFIT_DETAIL record
                var profitDetail = new ProfitDetail
                {
                    Ssn = employeeData.Ssn,
                    ProfitYear = (short)req.ProfitYear,
                    ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id,
                    Forfeiture = -req.ForfeitureAmount,
                    CommentTypeId = CommentType.Constants.Forfeit.Id,
                    Remark = remarkText,
                    TransactionDate = DateTimeOffset.UtcNow,
                    ModifiedAtUtc = DateTimeOffset.UtcNow
                };

                context.ProfitDetails.Add(profitDetail);

                // Get the PayProfit record to update
                var payProfit = await context.PayProfits
                    .FirstOrDefaultAsync(pp => pp.DemographicId == employeeData.Id && pp.ProfitYear == req.ProfitYear, cancellationToken);

                if (payProfit != null)
                {
                    if (isForfeit)
                    {
                        int wallClockYear = DateTime.Now.Year;
                        if (req.ProfitYear <= wallClockYear - 2)
                        {
                            throw new ArgumentException($"Cannot update profit year {req.ProfitYear}. Only current year ({wallClockYear}) and previous year ({wallClockYear - 1}) are allowed.");
                        }

                        // Determine live year set - normally just current year, but includes previous year for special cases (Year End)
                        var liveYearSet = req.ProfitYear == wallClockYear - 1 
                            ? new[] { wallClockYear - 1, wallClockYear }
                            : new[] { wallClockYear };

                        // For forfeit: Set PY_PS_ETVA to 0 and increment enrollment by 2
                        byte newEnrollmentId;
                        if (payProfit.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions)
                        {
                            newEnrollmentId = Enrollment.Constants.NewVestingPlanHasForfeitureRecords;
                        }
                        else if (payProfit.EnrollmentId == Enrollment.Constants.OldVestingPlanHasContributions)
                        {
                            newEnrollmentId = Enrollment.Constants.OldVestingPlanHasForfeitureRecords;
                        }
                        else
                        {
                            newEnrollmentId = Enrollment.Constants.NotEnrolled;
                        }

                        await context.PayProfits
                            .Where(pp => pp.DemographicId == employeeData.Id && liveYearSet.Contains(pp.ProfitYear))
                            .ExecuteUpdateAsync(p => p
                                .SetProperty(pp => pp.EnrollmentId, newEnrollmentId)
                                .SetProperty(pp => pp.Etva, 0)
                                .SetProperty(pp => pp.ModifiedAtUtc, DateTimeOffset.UtcNow),
                                cancellationToken);
                    }
                    else
                    {
                        // For un-forfeit: Recalculate PY_PS_ETVA and decrement enrollment by 2
                        byte newEnrollmentId;
                        
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

                // Return the updated entity to the frontend to update grid
                decimal startingBalance = vestingBalance.CurrentBalance;
                decimal forfeitureAmount = req.ForfeitureAmount;
                decimal netBalance = startingBalance - forfeitureAmount;
                decimal netVested = vestingBalance.VestedBalance;

                results.Add(new ForfeitureAdjustmentReportDetail
                {
                    DemographicId = employeeData.Id,
                    BadgeNumber = employeeData.BadgeNumber,
                    StartingBalance = startingBalance,
                    ForfeitureAmount = forfeitureAmount,
                    NetBalance = netBalance,
                    NetVested = netVested
                });
            }

            await context.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        return results;
    }
}


