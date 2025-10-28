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
/// Service for generating divorce reports with member account activity by profit year.
/// Uses TotalService for consistent profit code filtering and calculation logic.
/// </summary>
public class DivorceReportService : IDivorceReportService
{
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TotalService _totalService;

    public DivorceReportService(
        IProfitSharingDataContextFactory contextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService)
    {
        _contextFactory = contextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
    }

    public async Task<ReportResponseBase<DivorceReportResponse>> GetDivorceReportAsync(
        int memberId,
        StartAndEndDateRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _contextFactory.UseReadOnlyContext(async ctx =>
        {
            // Get member demographic information
            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, false);
            var demographic = await demographicQuery
                .FirstOrDefaultAsync(d => d.BadgeNumber == memberId, cancellationToken);

            if (demographic is null)
            {
                return new List<DivorceReportResponse>();
            }

            // Get all profit years for this member
            var profitYears = await ctx.ProfitDetails
                .TagWith($"DivorceReport-{demographic.BadgeNumber}")
                .Where(pd => pd.Ssn == demographic.Ssn)
                .Select(pd => pd.ProfitYear)
                .Distinct()
                .OrderBy(py => py)
                .ToListAsync(cancellationToken);

            if (!profitYears.Any())
            {
                return new List<DivorceReportResponse>();
            }

            var reportData = new List<DivorceReportResponse>();
            decimal cumulativeContributions = 0;
            decimal cumulativeWithdrawals = 0;
            decimal cumulativeDistributions = 0;

            // Process each year using TotalService calculations
            foreach (var year in profitYears.Where(y => y >= request.BeginningDate.Year && y <= request.EndingDate.Year))
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

                decimal yearContributions = yearTransactions.TotalContributions;
                decimal yearWithdrawals = yearTransactions.AllocationsTotal;
                decimal yearDistributions = yearTransactions.DistributionsTotal;
                decimal yearDividends = yearTransactions.TotalEarnings;
                decimal yearForfeitures = yearTransactions.ForfeitsTotal;

                // Accumulate totals
                cumulativeContributions += yearContributions;
                cumulativeWithdrawals += yearWithdrawals;
                cumulativeDistributions += yearDistributions;

                // Calculate ending balance for the year
                decimal endingBalance = yearTransactions.CurrentBalance;

                reportData.Add(new DivorceReportResponse
                {
                    BadgeNumber = demographic.BadgeNumber,
                    FullName = demographic.ContactInfo.FullName ?? string.Empty,
                    Ssn = demographic.Ssn.MaskSsn(),
                    ProfitYear = year,
                    TotalContributions = yearContributions,
                    TotalWithdrawals = yearWithdrawals,
                    TotalDistributions = yearDistributions,
                    TotalDividends = yearDividends,
                    TotalForfeitures = yearForfeitures,
                    EndingBalance = endingBalance,
                    CumulativeContributions = cumulativeContributions,
                    CumulativeWithdrawals = cumulativeWithdrawals,
                    CumulativeDistributions = cumulativeDistributions
                });
            }

            return reportData;
        }, cancellationToken);

        var dateRange = request.BeginningDate <= request.EndingDate
            ? (request.BeginningDate, request.EndingDate)
            : (request.EndingDate, request.BeginningDate);

        return new ReportResponseBase<DivorceReportResponse>
        {
            ReportName = "Divorce Report",
            ReportDate = DateTimeOffset.Now,
            StartDate = dateRange.Item1,
            EndDate = dateRange.Item2,
            Response = new PaginatedResponseDto<DivorceReportResponse>
            {
                Results = result,
                Total = result.Count
            }
        };
    }
}
