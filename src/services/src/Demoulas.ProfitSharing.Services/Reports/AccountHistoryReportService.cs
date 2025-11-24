using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Reporting.Reports;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
    private readonly IAppUser _user;
    private readonly ILogger<AccountHistoryReportService> _logger;

    public AccountHistoryReportService(
        IProfitSharingDataContextFactory contextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService,
        IAppUser user,
        ILogger<AccountHistoryReportService> logger)
    {
        _contextFactory = contextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
        _user = user;
        _logger = logger;
    }

    public async Task<AccountHistoryReportPaginatedResponse> GetAccountHistoryReportAsync(
        int memberId,
        AccountHistoryReportRequest request,
        CancellationToken cancellationToken)
    {
        var endYear = request.EndDate?.Year ?? DateTime.Today.Year;
        var startYear = request.StartDate?.Year ?? endYear - 3;

        var allYears = await _contextFactory.UseReadOnlyContext(async ctx =>
        {
            // Get member demographic information
            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, false);
            var demographic = await demographicQuery
                .TagWith($"AccountHistoryReport-Demographics-{memberId}")
                .FirstOrDefaultAsync(d => d.BadgeNumber == memberId, cancellationToken);

            if (demographic is null)
            {
                return new List<short>();
            }

            // Retrieve all profit years for this member within the date range
            return await ctx.ProfitDetails
                .TagWith($"AccountHistoryReport-Years-{demographic.BadgeNumber}")
                .Where(pd => pd.Ssn == demographic.Ssn &&
                             pd.ProfitYear >= startYear &&
                             pd.ProfitYear <= endYear)
                .Select(pd => pd.ProfitYear)
                .Distinct()
                .OrderBy(py => py)
                .ToListAsync(cancellationToken);


        }, cancellationToken);

        decimal previousCumulativeEndingBalance = 0;
        decimal previousCumulativeDistributions = 0;
        var reportData = new List<AccountHistoryReportResponse>();

        var tasks = new List<Task<AccountHistoryReportResponse>>();

        foreach (var year in allYears)
        {
            tasks.Add(_contextFactory.UseReadOnlyContext(async ctx =>
            {
                var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, false);
                var demographic = await demographicQuery
                    .TagWith($"AccountHistoryReport-Demographics-{memberId}")
                    .FirstOrDefaultAsync(d => d.BadgeNumber == memberId, cancellationToken);

                // Get cumulative totals from TotalService for this year
                var balanceSet = _totalService.GetTotalBalanceSet(ctx, year);
                var memberBalance = await balanceSet
                    .FirstOrDefaultAsync(b => b.Ssn == demographic!.Ssn, cancellationToken);

                var distributions = _totalService.GetTotalDistributions(ctx, year);
                var memberDistributions = await distributions
                    .FirstOrDefaultAsync(d => d.Ssn == demographic!.Ssn, cancellationToken);

                // Get vesting balance for this year
                var vestingBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(
                    SearchBy.BadgeNumber, demographic!.BadgeNumber, year, cancellationToken);

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

                var yearVestedBalance = vestingBalance?.VestedBalance ?? 0;
                return new AccountHistoryReportResponse
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
                    VestedBalance = yearVestedBalance
                };
            }, cancellationToken));
        }

        var results = await Task.WhenAll(tasks);
        reportData.AddRange(results);

        // Apply sorting based on request parameters
        var sortBy = request.SortBy ?? "ProfitYear";
        var isSortDescending = request.IsSortDescending ?? true;
        var sortedResult = ApplySorting(reportData, sortBy, isSortDescending).ToList();

        // Calculate cumulative totals across all results
        var totalContributions = sortedResult.Sum(r => r.Contributions);
        var totalEarnings = sortedResult.Sum(r => r.Earnings);
        var totalForfeitures = sortedResult.Sum(r => r.Forfeitures);
        var totalWithdrawals = sortedResult.Sum(r => r.Withdrawals);
        // TotalVestedBalance is from the highest profit year (last in the unsorted chronological list)
        var totalVestedBalance = reportData.LastOrDefault()?.VestedBalance ?? 0;

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

    /// <summary>
    /// Generates a PDF report for the account history data.
    /// </summary>
    /// <param name="memberId">The member badge number</param>
    /// <param name="request">The account history report request with date range and sorting</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Memory stream containing the PDF report</returns>
    public async Task<MemoryStream> GeneratePdfAsync(
        int memberId,
        AccountHistoryReportRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Generating PDF account history report for member badge {BadgeNumber}",
            memberId);

        try
        {
            // Get the full report data (no pagination for PDF)
            var fullRequest = request with { Skip = 0, Take = short.MaxValue };
            var reportData = await GetAccountHistoryReportAsync(memberId, fullRequest, cancellationToken);

            // Extract member profile from the first response item
            var responseList = reportData.Response.Results.ToList();
            var firstResponse = responseList.FirstOrDefault();
            if (firstResponse is null)
            {
                _logger.LogWarning(
                    "No account history data found for member badge {BadgeNumber}",
                    memberId);
                throw new InvalidOperationException($"No account history data found for member {memberId}");
            }

            // Create the PDF report document
            var memberProfile = new AccountHistoryPdfReport.MemberProfileInfo
            {
                FullName = firstResponse.FullName,
                BadgeNumber = firstResponse.BadgeNumber,
                MaskedSsn = firstResponse.Ssn
            };

            // Ensure cumulative totals are provided (initialize if null)
            var cumulativeTotals = reportData.CumulativeTotals ?? new AccountHistoryReportTotals();

            var pdfReport = new AccountHistoryPdfReport(
                memberProfile,
                responseList,
                cumulativeTotals,
                reportData.StartDate,
                reportData.EndDate,
                _user.UserName ?? "Member");

            // Generate PDF bytes and create memory stream
            byte[] pdfBytes = pdfReport.GeneratePdf();
            var pdfStream = new MemoryStream(pdfBytes, writable: false);

            _logger.LogInformation(
                "Successfully generated PDF account history report for member badge {BadgeNumber}, size: {FileSize} bytes",
                memberId,
                pdfStream.Length);

            return pdfStream;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(
                ex,
                "Invalid operation while generating PDF account history report for member badge {BadgeNumber}: {ErrorMessage}",
                memberId,
                ex.Message);
            throw new InvalidOperationException(
                $"Unable to generate PDF account history report for member {memberId}: {ex.Message}",
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error generating PDF account history report for member badge {BadgeNumber}: {ErrorMessage}",
                memberId,
                ex.Message);
            throw new InvalidOperationException(
                $"Failed to generate PDF account history report for member {memberId}: {ex.Message}",
                ex);
        }
    }
}
