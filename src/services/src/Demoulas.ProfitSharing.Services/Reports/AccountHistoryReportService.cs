using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

/// <summary>
/// Service for generating account history reports with member account activity by profit year.
/// Supports pagination and sorting of results.
/// Uses ProfitDetails with proper ProfitCodeId-based field interpretation for accurate aggregation.
/// </summary>
public class AccountHistoryReportService : IAccountHistoryReportService
{
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly IDemographicReaderService _demographicReaderService;

    public AccountHistoryReportService(
        IProfitSharingDataContextFactory contextFactory,
        IDemographicReaderService demographicReaderService)
    {
        _contextFactory = contextFactory;
        _demographicReaderService = demographicReaderService;
    }

    public async Task<AccountHistoryReportPaginatedResponse> GetAccountHistoryReportAsync(
        int memberId,
        AccountHistoryReportRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _contextFactory.UseReadOnlyContext(async ctx =>
        {
            // Get member demographic information
            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, false);
            var demographic = await demographicQuery
                .TagWith($"AccountHistoryReport-Demographics-{memberId}")
                .FirstOrDefaultAsync(d => d.BadgeNumber == memberId, cancellationToken);

            if (demographic is null)
            {
                return new List<AccountHistoryReportResponse>();
            }

            // Get all profit transactions for this member from ProfitDetails
            // ProfitDetails contains raw transaction data with ProfitCodeId which maps to transaction type
            var startYear = request.StartDate?.Year ?? 2007;
            var endYear = request.EndDate?.Year ?? DateTime.Today.Year;

            var allTransactions = await ctx.ProfitDetails
                .TagWith($"AccountHistoryReport-Transactions-{demographic.BadgeNumber}")
                .Where(pd => pd.Ssn == demographic.Ssn &&
                            pd.ProfitYear >= startYear &&
                            pd.ProfitYear <= endYear)
                .ToListAsync(cancellationToken);

            if (!allTransactions.Any())
            {
                return new List<AccountHistoryReportResponse>();
            }

            var reportData = new List<AccountHistoryReportResponse>();

            // Group transactions by profit year and aggregate by profit code
            var transactionsByYear = allTransactions
                .GroupBy(t => t.ProfitYear)
                .OrderBy(g => g.Key);

            foreach (var yearGroup in transactionsByYear)
            {
                var year = yearGroup.Key;
                var yearTransactions = yearGroup.ToList();

                // Get payment profit codes (1,2,3,5,9) where Forfeiture field stores payment/payout amounts
                var paymentProfitCodes = ProfitDetailExtensions.GetProfitCodesForBalanceCalc();

                var totalContributions = yearTransactions
                    .Sum(t => t.Contribution);

                var totalEarnings = yearTransactions
                    .Sum(t => t.Earnings);

                // Forfeitures: Only for profit codes NOT in payment codes (i.e., profitCodeId=0,2)
                // For profitCodeId=2, Forfeiture field contains the actual forfeiture amount
                var totalForfeitures = yearTransactions
                    .Where(t => !paymentProfitCodes.Contains(t.ProfitCodeId))
                    .Sum(t => t.Forfeiture);

                // Withdrawals: For payment profit codes (1,3,9), the Forfeiture field contains payment amounts
                // profitCodeId=1: Partial withdrawal
                // profitCodeId=3: Direct payment
                // profitCodeId=9: 100% vested payment
                // NOTE: profitCodeId=5 (QDRO to beneficiaries) is in paymentProfitCodes but NOT counted as employee withdrawal
                var totalWithdrawals = yearTransactions
                    .Where(t => t.ProfitCodeId is 1 or 3 or 9)  // Employee payouts only, exclude QDRO (5)
                    .Sum(t => t.Forfeiture);  // For these codes, Forfeiture field = payment amount

                // Ending balance: Net of all transactions for this year
                var endingBalance = totalContributions + totalEarnings - totalForfeitures - totalWithdrawals;

                reportData.Add(new AccountHistoryReportResponse
                {
                    BadgeNumber = demographic.BadgeNumber,
                    FullName = demographic.ContactInfo.FullName ?? string.Empty,
                    Ssn = demographic.Ssn.MaskSsn(),
                    ProfitYear = year,
                    Contributions = totalContributions,
                    Earnings = totalEarnings,
                    Forfeitures = totalForfeitures,
                    Withdrawals = totalWithdrawals,
                    EndingBalance = endingBalance
                });
            }

            return reportData;
        }, cancellationToken);

        // Apply sorting based on request parameters
        var sortBy = request.SortBy ?? "ProfitYear";
        var isSortDescending = request.IsSortDescending ?? true;
        var sortedResult = ApplySorting(result, sortBy, isSortDescending).ToList();

        // Calculate cumulative totals across all results
        var totalContributions = sortedResult.Sum(r => r.Contributions);
        var totalEarnings = sortedResult.Sum(r => r.Earnings);
        var totalForfeitures = sortedResult.Sum(r => r.Forfeitures);
        var totalWithdrawals = sortedResult.Sum(r => r.Withdrawals);
        var cumulativeBalance = sortedResult.LastOrDefault()?.EndingBalance ?? 0m;

        // Calculate total count before pagination
        var totalCount = sortedResult.Count;

        // Apply pagination
        var skip = request.Skip ?? 0;
        var take = request.Take ?? 25;
        var paginatedResult = sortedResult
            .Skip(skip)
            .Take(take)
            .ToList();

        var startDate = request.StartDate ?? new DateOnly(2007, 1, 1);
        var endDate = request.EndDate ?? DateOnly.FromDateTime(DateTime.Today);
        var dateRange = startDate <= endDate
            ? (startDate, endDate)
            : (endDate, startDate);

        var paginatedResponse = new AccountHistoryReportPaginatedResponse()
        {
            ReportName = "Account History Report",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = dateRange.Item1,
            EndDate = dateRange.Item2,
            Response = new PaginatedResponseDto<AccountHistoryReportResponse>
            {
                Results = paginatedResult,
                Total = totalCount
            },
            CumulativeTotals = new AccountHistoryReportTotals
            {
                TotalContributions = totalContributions,
                TotalEarnings = totalEarnings,
                TotalForfeitures = totalForfeitures,
                TotalWithdrawals = totalWithdrawals,
                CumulativeBalance = cumulativeBalance
            }
        };

        // Note: Cumulative totals (total contributions, earnings, forfeitures, withdrawals, balance)
        // are calculated from all sorted results before pagination. The UI reads these values
        // from the AccountHistoryReportPaginatedResponse.CumulativeTotals property
        return paginatedResponse;
    }

    /// <summary>
    /// Applies sorting to the account history report data.
    /// </summary>
    /// <param name="data">The data to sort</param>
    /// <param name="sortBy">The field to sort by (e.g., "ProfitYear")</param>
    /// <param name="descending">Whether to sort in descending order</param>
    /// <returns>Sorted IEnumerable of AccountHistoryReportResponse</returns>
    private static IEnumerable<AccountHistoryReportResponse> ApplySorting(
        List<AccountHistoryReportResponse> data,
        string sortBy,
        bool descending)
    {
        var sorted = sortBy.ToLowerInvariant() switch
        {
            "profityear" => descending
                ? data.OrderByDescending(x => x.ProfitYear)
                : data.OrderBy(x => x.ProfitYear),
            "badgenumber" => descending
                ? data.OrderByDescending(x => x.BadgeNumber)
                : data.OrderBy(x => x.BadgeNumber),
            "fullname" => descending
                ? data.OrderByDescending(x => x.FullName)
                : data.OrderBy(x => x.FullName),
            "contributions" => descending
                ? data.OrderByDescending(x => x.Contributions)
                : data.OrderBy(x => x.Contributions),
            "earnings" => descending
                ? data.OrderByDescending(x => x.Earnings)
                : data.OrderBy(x => x.Earnings),
            "endingbalance" => descending
                ? data.OrderByDescending(x => x.EndingBalance)
                : data.OrderBy(x => x.EndingBalance),
            _ => descending
                ? data.OrderByDescending(x => x.ProfitYear)
                : data.OrderBy(x => x.ProfitYear)
        };

        return sorted;
    }
}
