using System.Linq;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

/// <summary>
/// Service for generating account history reports with member account activity by profit year.
/// Supports pagination and sorting of results.
/// Uses TotalService for consistent profit code filtering and calculation logic.
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

    public async Task<ReportResponseBase<AccountHistoryReportResponse>> GetAccountHistoryReportAsync(
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

            // Get all profit years for this member
            var profitYears = await ctx.ProfitDetails
                .TagWith($"AccountHistoryReport-ProfitYears-{demographic.BadgeNumber}")
                .Where(pd => pd.Ssn == demographic.Ssn)
                .Select(pd => pd.ProfitYear)
                .Distinct()
                .OrderBy(py => py)
                .ToListAsync(cancellationToken);

            if (!profitYears.Any())
            {
                return new List<AccountHistoryReportResponse>();
            }

            var reportData = new List<AccountHistoryReportResponse>();

            // Process each year using TotalService calculations
            foreach (var year in profitYears.Where(y => y >= (request.StartDate?.Year ?? 2007) && y <= (request.EndDate?.Year ?? DateTime.Today.Year)))
            {
                // Get transactions for this year using TotalService's Oracle query which has all profit code filtering built in
                var yearTransactions = await _totalService
                    .GetTransactionsBySsnForProfitYearForOracle(ctx, year)
                    .Where(t => t.Ssn == demographic.Ssn)
                    .FirstOrDefaultAsync(cancellationToken);

                if (yearTransactions is null)
                {
                    continue;
                }

                // Map to simplified response showing only essential fields
                reportData.Add(new AccountHistoryReportResponse
                {
                    BadgeNumber = demographic.BadgeNumber,
                    FullName = demographic.ContactInfo.FullName ?? string.Empty,
                    Ssn = demographic.Ssn.MaskSsn(),
                    ProfitYear = year,
                    Contributions = yearTransactions.TotalContributions,
                    Earnings = yearTransactions.TotalEarnings,
                    Forfeitures = yearTransactions.ForfeitsTotal,
                    Withdrawals = yearTransactions.AllocationsTotal,
                    EndingBalance = yearTransactions.CurrentBalance
                });
            }

            return reportData;
        }, cancellationToken);

        // Apply sorting based on request parameters
        var sortBy = request.SortBy ?? "ProfitYear";
        var isSortDescending = request.IsSortDescending ?? true;
        var sortedResult = ApplySorting(result, sortBy, isSortDescending).ToList();

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

        return new ReportResponseBase<AccountHistoryReportResponse>
        {
            ReportName = "Account History Report",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = dateRange.Item1,
            EndDate = dateRange.Item2,
            Response = new PaginatedResponseDto<AccountHistoryReportResponse>
            {
                Results = paginatedResult,
                Total = totalCount
            }
        };
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
