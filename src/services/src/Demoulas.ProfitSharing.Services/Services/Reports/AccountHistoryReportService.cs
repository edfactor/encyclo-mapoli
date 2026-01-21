using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Reporting.Reports;
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
    private readonly IMasterInquiryService _employeeLookupService;
    private readonly ILogger<AccountHistoryReportService> _logger;
    private readonly IProfitSharingAuditService _profitSharingAuditService;

    public AccountHistoryReportService(
        IProfitSharingDataContextFactory contextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService,
        IAppUser user,
        IMasterInquiryService employeeLookupService,
        ILogger<AccountHistoryReportService> logger,
        IProfitSharingAuditService profitSharingAuditService)
    {
        _contextFactory = contextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
        _user = user;
        _employeeLookupService = employeeLookupService;
        _logger = logger;
        _profitSharingAuditService = profitSharingAuditService;
    }

    public Task<AccountHistoryReportPaginatedResponse> GetAccountHistoryReportAsync(
        int memberId,
        AccountHistoryReportRequest request,
        CancellationToken cancellationToken)
    {
        var endYear = request.EndDate?.Year ?? DateTime.Today.Year;
        var startYear = request.StartDate?.Year ?? endYear - 3;

        // Single database context for initial queries, then parallel execution for per-year data
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            // Get member demographic information (single query)
            var demographicQuery = await _demographicReaderService.BuildDemographicQueryAsync(ctx, false);
            var demographic = await demographicQuery
                .TagWith($"AccountHistoryReport-Demographics-{memberId}")
                .FirstOrDefaultAsync(d => d.BadgeNumber == memberId, cancellationToken);

            if (demographic is null)
            {
                return CreateEmptyResponse(request, startYear);
            }

            var ssn = demographic.Ssn;
            var badgeNumber = demographic.BadgeNumber;
            var demographicId = demographic.Id;

            // Get all profit years for this member - apply sorting to determine which years we need
            var sortBy = request.SortBy ?? "ProfitYear";
            var isSortDescending = request.IsSortDescending ?? true;
            var skip = request.Skip ?? 0;
            var take = request.Take ?? 25;

            // For year-based sorting, we can optimize by only fetching the years we need
            // For other sorting, we need all years to sort properly
            var isYearBasedSort = sortBy.Equals("profityear", StringComparison.OrdinalIgnoreCase);

            IQueryable<short> yearsQuery = ctx.ProfitDetails
                .TagWith($"AccountHistoryReport-Years-{badgeNumber}")
                .Where(pd => pd.Ssn == ssn &&
                             pd.ProfitYear >= startYear &&
                             pd.ProfitYear <= endYear)
                .Select(pd => pd.ProfitYear)
                .Distinct();

            // Get total count for pagination
            var totalYearCount = await yearsQuery.CountAsync(cancellationToken);

            if (totalYearCount == 0)
            {
                return CreateEmptyResponse(request, startYear);
            }

            List<short> yearsToFetch;
            if (isYearBasedSort)
            {
                // Optimize: only fetch years needed for current page
                yearsQuery = isSortDescending
                    ? yearsQuery.OrderByDescending(y => y)
                    : yearsQuery.OrderBy(y => y);

                yearsToFetch = await yearsQuery
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync(cancellationToken);
            }
            else
            {
                // For non-year sorting, we need all data to sort correctly
                // But still limit to a reasonable max to prevent excessive queries
                const int maxYearsToFetch = 100;
                yearsToFetch = await yearsQuery
                    .OrderBy(y => y)
                    .Take(maxYearsToFetch)
                    .ToListAsync(cancellationToken);
            }

            // Execute per-year queries in parallel for better performance
            // Each year's contributions/earnings/forfeitures are computed in the database
            var tasks = yearsToFetch.Select(year => FetchYearDataAsync(
                year,
                ssn,
                badgeNumber,
                demographicId,
                cancellationToken)).ToList();

            var reportData = (await Task.WhenAll(tasks)).ToList();
            var latestProfitYear = reportData.Count == 0
                ? (short)startYear
                : reportData.Max(r => r.ProfitYear);
            var latestVestedBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(
                SearchBy.BadgeNumber,
                badgeNumber,
                latestProfitYear,
                cancellationToken);
            var totalVestedBalance = latestVestedBalance?.VestedBalance ?? 0m;

            // Build response - for year-based sort, data is already in correct order and paginated
            if (isYearBasedSort)
            {
                // Calculate cumulative totals from the fetched data
                var cumulativeTotals = new AccountHistoryReportTotals
                {
                    TotalContributions = reportData.Sum(r => r.Contributions),
                    TotalEarnings = reportData.Sum(r => r.Earnings),
                    TotalForfeitures = reportData.Sum(r => r.Forfeitures),
                    TotalWithdrawals = reportData.Sum(r => r.Withdrawals),
                    TotalVestedBalance = totalVestedBalance
                };

                return BuildPaginatedResponsePreSorted(reportData, request, startYear, totalYearCount, cumulativeTotals);
            }

            // For non-year sorting, apply sorting and pagination now
            return BuildPaginatedResponse(reportData, request, startYear, totalVestedBalance);
        }, cancellationToken);
    }

    /// <summary>
    /// Fetches year-specific data (balances, distributions, vesting) in a separate context for parallel execution.
    /// Each year runs in its own context, enabling parallel execution across years.
    /// MUST use EmbeddedSqlService.GetTransactionsBySsnForProfitYearForOracle() for validated aggregations.
    /// </summary>
    private Task<AccountHistoryReportResponse> FetchYearDataAsync(
        short year,
        int ssn,
        int badgeNumber,
        int demographicId,
        CancellationToken cancellationToken)
    {
        // Execute balance/distribution queries sequentially within this context
        // (EF Core doesn't allow concurrent operations on the same context)
        // Parallelism is achieved at the year level - each year has its own context
        return _contextFactory.UseReadOnlyContext(async ctx =>
        {
            // Use validated SQL formulas from EmbeddedSqlService for year aggregations
            // This ensures we use the SME-approved formulas, not replicated C# logic
            var yearRollup = await _totalService.GetTransactionsBySsnForProfitYearForOracle(ctx, year)
                .FirstOrDefaultAsync(r => r.Ssn == ssn, cancellationToken);

            var memberBalance = await _totalService.GetTotalBalanceSet(ctx, year)
                .FirstOrDefaultAsync(b => b.Ssn == ssn, cancellationToken);

            // PS-2424: Use GetYearlyDistributions for yearly amounts (not cumulative)
            var memberDistributions = await _totalService.GetYearlyDistributions(ctx, year)
                .FirstOrDefaultAsync(d => d.Ssn == ssn, cancellationToken);

            // GetVestingBalanceForSingleMemberAsync creates its own context internally
            var vestingBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(
                SearchBy.BadgeNumber, badgeNumber, year, cancellationToken);

            return new AccountHistoryReportResponse
            {
                Id = demographicId,
                BadgeNumber = badgeNumber,
                ProfitYear = year,
                Contributions = yearRollup?.TotalContributions ?? 0,
                Earnings = yearRollup?.TotalEarnings ?? 0,
                Forfeitures = yearRollup?.TotalForfeitures ?? 0,
                Withdrawals = memberDistributions?.TotalAmount ?? 0,
                EndingBalance = memberBalance?.TotalAmount ?? 0,
            };
        }, cancellationToken);
    }

    /// <summary>
    /// Creates an empty response when no data is found.
    /// </summary>
    private static AccountHistoryReportPaginatedResponse CreateEmptyResponse(
        AccountHistoryReportRequest request,
        int startYear)
    {
        return new AccountHistoryReportPaginatedResponse
        {
            ReportName = "Account History Report",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = request.StartDate ?? new DateOnly(startYear, 1, 1),
            EndDate = request.EndDate ?? DateOnly.FromDateTime(DateTime.Today),
            Response = new PaginatedResponseDto<AccountHistoryReportResponse>
            {
                Results = [],
                Total = 0
            },
            CumulativeTotals = new AccountHistoryReportTotals()
        };
    }

    /// <summary>
    /// Builds the paginated response from report data.
    /// </summary>
    private static AccountHistoryReportPaginatedResponse BuildPaginatedResponse(
        List<AccountHistoryReportResponse> reportData,
        AccountHistoryReportRequest request,
        int startYear,
        decimal totalVestedBalance)
    {
        // Apply sorting based on request parameters
        var sortBy = request.SortBy ?? "ProfitYear";
        var isSortDescending = request.IsSortDescending ?? true;
        var sortedResult = ApplySorting(reportData, sortBy, isSortDescending).ToList();

        // Calculate cumulative totals across all results
        var totalContributions = sortedResult.Sum(r => r.Contributions);
        var totalEarnings = sortedResult.Sum(r => r.Earnings);
        var totalForfeitures = sortedResult.Sum(r => r.Forfeitures);
        var totalWithdrawals = sortedResult.Sum(r => r.Withdrawals);

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

        return new AccountHistoryReportPaginatedResponse
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
    }

    /// <summary>
    /// Builds the paginated response when data is already fetched for the correct page.
    /// This is used when sorting by year and we only fetch the years needed for the current page.
    /// </summary>
    /// <param name="reportData">Report data already limited to the current page</param>
    /// <param name="request">The request with pagination and date range info</param>
    /// <param name="startYear">The default start year for date range</param>
    /// <param name="totalYearCount">The total count of years in the full date range (for pagination)</param>
    /// <param name="cumulativeTotals">Pre-calculated cumulative totals across all years</param>
    private static AccountHistoryReportPaginatedResponse BuildPaginatedResponsePreSorted(
        List<AccountHistoryReportResponse> reportData,
        AccountHistoryReportRequest request,
        int startYear,
        int totalYearCount,
        AccountHistoryReportTotals cumulativeTotals)
    {
        var startDate = request.StartDate ?? new DateOnly(startYear, 1, 1);
        var endDate = request.EndDate ?? DateOnly.FromDateTime(DateTime.Today);
        var dateRange = startDate <= endDate
            ? (startDate, endDate)
            : (endDate, startDate);

        return new AccountHistoryReportPaginatedResponse
        {
            ReportName = "Account History Report",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = dateRange.Item1,
            EndDate = dateRange.Item2,
            Response = new PaginatedResponseDto<AccountHistoryReportResponse>
            {
                Results = reportData,
                Total = totalYearCount
            },
            CumulativeTotals = cumulativeTotals
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

            var employee = await _employeeLookupService.GetMemberAsync(new MasterInquiryMemberRequest()
            {
                Id = firstResponse.Id,
                MemberType = 1,
                ProfitYear = (short)reportData.EndDate.Year
            }, cancellationToken);

            // Create the PDF report document
            var memberProfile = new AccountHistoryPdfReport.MemberProfileInfo
            {
                FullName = employee?.FullName ?? string.Empty,
                BadgeNumber = employee?.BadgeNumber ?? 0,
                MaskedSsn = employee?.Ssn ?? string.Empty,
                Address = employee?.Address,
                City = employee?.AddressCity,
                State = employee?.AddressState,
                ZipCode = employee?.AddressZipCode,
                Phone = employee?.PhoneNumber,
                DateOfBirth = employee?.DateOfBirth,
                EmploymentStatus = employee?.EmploymentStatus,
                HireDate = employee?.HireDate,
                StoreNumber = employee?.StoreNumber,
                TerminationDate = employee?.TerminationDate
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

            // Log audit event for PDF download tracking (PS-2284: Ensure audit tracking for Account History download)
            await _profitSharingAuditService.LogSensitiveDataAccessAsync(
                operationName: "Account History PDF Download",
                tableName: "AccountHistory",
                primaryKey: $"Badge:{memberId}",
                details: $"BadgeNumber: {memberId}, Profit Year Range: {reportData.StartDate.Year} - {reportData.EndDate.Year}, Records: {reportData.Response.Total}",
                cancellationToken);

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
