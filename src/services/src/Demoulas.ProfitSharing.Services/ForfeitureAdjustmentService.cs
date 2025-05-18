using System.Data.SqlTypes;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.Common.Contracts.Contracts.Response;
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
        public int Ssn { get; init; }
        public int BadgeNumber { get; init; }
        public int StoreNumber { get; init; }
    }

    public Task<ForfeitureAdjustmentReportResponse> GetForfeitureAdjustmentReportAsync(ForfeitureAdjustmentRequest req, CancellationToken cancellationToken = default)
    {
        // Default to return if any need to short circuit
        var response = new ForfeitureAdjustmentReportResponse
        {
            ReportName = ForfeitureAdjustmentReportResponse.REPORT_NAME,
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = SqlDateTime.MinValue.Value.ToDateOnly(),
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
            int priorProfitYear = req.ProfitYear - 1;
            decimal startingBalance = await _embeddedSqlService.GetTotalBalanceAlt(context, (short)priorProfitYear)
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
                ClientNumber = employeeData.StoreNumber > 0 ? employeeData.StoreNumber : 0,
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


    private static async Task<EmployeeInfo?> FindEmployeeAsync(IProfitSharingDbContext context,
        ForfeitureAdjustmentRequest req, CancellationToken cancellationToken)
    {
        // If SSN is provided, use that for lookup
        if (req.SSN.HasValue)
        {
            return await context.Demographics
                .Where(d => d.Ssn == req.SSN.Value)
                .Where(d => context.PayProfits.Any(pp => pp.DemographicId == d.Id && pp.ProfitYear == req.ProfitYear))
                .Select(d => new EmployeeInfo
                {
                    Ssn = d.Ssn,
                    BadgeNumber = d.BadgeNumber,
                    StoreNumber = d.StoreNumber
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
        // Otherwise, if Badge is provided, use that for lookup
        else if (req.Badge.HasValue)
        {
            return await context.Demographics
                .Where(d => d.BadgeNumber == req.Badge.Value)
                .Where(d => context.PayProfits.Any(pp => pp.DemographicId == d.Id && pp.ProfitYear == req.ProfitYear))
                .Select(d => new EmployeeInfo
                {
                    Ssn = d.Ssn,
                    BadgeNumber = d.BadgeNumber,
                    StoreNumber = d.StoreNumber
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
                    PayProfit = context.PayProfits
                        .FirstOrDefault(pp => pp.DemographicId == d.Id && pp.ProfitYear == req.ProfitYear)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (employeeData == null)
            {
                throw new ArgumentException($"Employee with badge number {req.BadgeNumber} not found");
            }

            if (employeeData.PayProfit == null)
            {
                throw new ArgumentException($"No profit sharing data found for employee with badge number {req.BadgeNumber} for year {req.ProfitYear}");
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

            // Determine if this is a forfeit or un-forfeit operation
            bool isForfeit = req.ForfeitureAmount > 0;

            // From docs: "From the screen in figure 2, if you enter the value in #17 to box #12 and hit enter, you will create a PROFIT_DETAIL record.
            // When the value is negative the record has UN-FORFEIT in the PROFIT_CMNT field and when the value is positive the PROFIT_CMNT field is FORFEIT."
            string remarkText = isForfeit ? "FORFEIT" : "UN-FORFEIT";

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
                TransactionDate = DateTimeOffset.Now
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
                    // For forfeit: Set PY_PS_ETVA to 0 and increment enrollment by 2
                    payProfit.Etva = 0;  // PY_PS_ETVA

                    // Update EnrollmentId using ExecuteUpdateAsync - read-only resource constraints otherwise
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
                        .Where(pp => pp.DemographicId == employeeData.Id && pp.ProfitYear == req.ProfitYear)
                        .ExecuteUpdateAsync(p => p
                            .SetProperty(pp => pp.EnrollmentId, newEnrollmentId)
                            .SetProperty(pp => pp.LastUpdate, DateTime.Now), 
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
                            .SetProperty(pp => pp.LastUpdate, DateTimeOffset.UtcNow),
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
                ClientNumber = req.ClientNumber,
                BadgeNumber = req.BadgeNumber,
                StartingBalance = startingBalance,
                ForfeitureAmount = forfeitureAmount,
                NetBalance = netBalance,
                NetVested = netVested
            };
        }, cancellationToken);

        return result ?? new ForfeitureAdjustmentReportDetail
        {
            ClientNumber = 0,
            BadgeNumber = 0,
            StartingBalance = 0,
            ForfeitureAmount = 0,
            NetBalance = 0,
            NetVested = 0
        };
    }
}


