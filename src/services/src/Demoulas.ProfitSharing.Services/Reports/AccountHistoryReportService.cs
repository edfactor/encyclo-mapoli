using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.MasterInquiry;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

/// <summary>
/// Service for generating account history reports with member account activity by profit year.
/// Supports pagination and sorting of results.
/// Leverages TotalService for centralized profit-sharing calculations.
/// </summary>
public class AccountHistoryReportService : IAccountHistoryReportService
{
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TotalService _totalService;

    public AccountHistoryReportService(
        IProfitSharingDataContextFactory contextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService)
    {
        _contextFactory = contextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
    }

    public async Task<AccountHistoryReportPaginatedResponse> GetAccountHistoryReportAsync(
        int memberId,
        AccountHistoryReportRequest request,
        CancellationToken cancellationToken)
    {
        var endYear = request.EndDate?.Year ?? DateTime.Today.Year;
        var startYear = request.StartDate?.Year ?? endYear - 3;

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

            // Retrieve all profit years for this member within the date range
            var allYears = await ctx.ProfitDetails
                .TagWith($"AccountHistoryReport-Years-{demographic.BadgeNumber}")
                .Where(pd => pd.Ssn == demographic.Ssn &&
                            pd.ProfitYear >= startYear &&
                            pd.ProfitYear <= endYear)
                .Select(pd => pd.ProfitYear)
                .Distinct()
                .OrderBy(py => py)
                .ToListAsync(cancellationToken);

            if (!allYears.Any())
            {
                return new List<AccountHistoryReportResponse>();
            }

            var reportData = new List<AccountHistoryReportResponse>();
            decimal previousCumulativeEndingBalance = 0;
            decimal previousCumulativeDistributions = 0;

            foreach (var year in allYears)
            {
                // Get cumulative totals from TotalService for this year
                var balanceSet = _totalService.GetTotalBalanceSet(ctx, year);
                var memberBalance = await balanceSet
                    .FirstOrDefaultAsync(b => b.Ssn == demographic.Ssn, cancellationToken);

                var distributions = _totalService.GetTotalDistributions(ctx, year);
                var memberDistributions = await distributions
                    .FirstOrDefaultAsync(d => d.Ssn == demographic.Ssn, cancellationToken);

                // Get vesting balance for this year
                var vestingBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(
                    SearchBy.BadgeNumber, demographic.BadgeNumber, (short)year, cancellationToken);

                // Get year-specific profit details for contributions, earnings, and forfeitures
                var yearProfitDetails = await ctx.ProfitDetails
                    .TagWith($"AccountHistoryReport-ProfitDetails-{demographic.BadgeNumber}-{year}")
                    .Where(pd => pd.Ssn == demographic.Ssn && pd.ProfitYear == year)
                    .ToListAsync(cancellationToken);

                // Use shared extension methods for consistent aggregation logic
                (decimal yearContributions, decimal yearEarnings, decimal yearForfeitures) =
                    ProfitDetailExtensions.AggregateAllProfitValues(yearProfitDetails);

                // Current cumulative totals
                decimal currentCumulativeEndingBalance = memberBalance?.TotalAmount ?? 0;
                decimal currentCumulativeDistributions = memberDistributions?.TotalAmount ?? 0;

                // Year-over-year calculations: difference from previous year
                decimal yearEndingBalance = currentCumulativeEndingBalance - previousCumulativeEndingBalance;
                decimal yearWithdrawals = currentCumulativeDistributions - previousCumulativeDistributions;

                reportData.Add(new AccountHistoryReportResponse
                {
                    Id = demographic.Id,
                    BadgeNumber = demographic.BadgeNumber,
                    FullName = demographic.ContactInfo.FullName ?? string.Empty,
                    Ssn = demographic.Ssn.MaskSsn(),
                    ProfitYear = year,
                    Contributions = yearContributions,
                    Earnings = yearEarnings,
                    Forfeitures = yearForfeitures,
                    Withdrawals = yearWithdrawals,
                    EndingBalance = currentCumulativeEndingBalance,
                    VestedBalance = vestingBalance?.VestedBalance ?? 0
                });

                previousCumulativeEndingBalance = currentCumulativeEndingBalance;
                previousCumulativeDistributions = currentCumulativeDistributions;
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
        var totalVestedBalance = sortedResult.Sum(r => r.VestedBalance);

        // Calculate total count before pagination
        var totalCount = sortedResult.Count;

        // Apply pagination
        var skip = request.Skip ?? 0;
        var take = request.Take ?? 25;
        var paginatedResult = sortedResult
            .Skip(skip)
            .Take(take)
            .ToList();

        var startDate = request.StartDate ?? new DateOnly(startYear, 1, 1);
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
                TotalVestedBalance = totalVestedBalance
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
