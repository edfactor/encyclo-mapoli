using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

/// <summary>
/// Service for generating divorce reports with member account activity by profit year.
/// </summary>
public class DivorceReportService : IDivorceReportService
{
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly IDemographicReaderService _demographicReaderService;

    public DivorceReportService(
        IProfitSharingDataContextFactory contextFactory,
        IDemographicReaderService demographicReaderService)
    {
        _contextFactory = contextFactory;
        _demographicReaderService = demographicReaderService;
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

            // Query profit details for the member within the date range
            var profitDetails = await ctx.ProfitDetails
                .Where(pd => pd.Ssn == demographic.Ssn)
                .OrderBy(pd => pd.ProfitYear)
                .ToListAsync(cancellationToken);

            if (!profitDetails.Any())
            {
                return new List<DivorceReportResponse>();
            }

            // Group by profit year and calculate totals
            var reportData = new List<DivorceReportResponse>();
            long cumulativeContributions = 0;
            long cumulativeWithdrawals = 0;
            long cumulativeDistributions = 0;

            foreach (var yearGroup in profitDetails.GroupBy(pd => pd.ProfitYear).OrderBy(g => g.Key))
            {
                var year = yearGroup.Key;
                var yearDetails = yearGroup.ToList();

                // Calculate year totals by profit code
                long yearContributions = 0;
                long yearWithdrawals = 0;
                long yearDistributions = 0;
                long yearDividends = 0;
                long yearForfeitures = 0;

                foreach (var detail in yearDetails)
                {
                    var contributionInCents = (long)(detail.Contribution * 100);
                    var earningsInCents = (long)(detail.Earnings * 100);
                    var forfeitureInCents = (long)(detail.Forfeiture * 100);

                    // Categorize by profit code
                    // This is a simplified approach - actual profit codes should determine categorization
                    switch (detail.ProfitCodeId)
                    {
                        case (byte)'C': // Contribution
                            yearContributions += contributionInCents;
                            yearDividends += earningsInCents;
                            break;
                        case (byte)'W': // Withdrawal/Loan
                            yearWithdrawals += contributionInCents;
                            break;
                        case (byte)'D': // Distribution
                            yearDistributions += contributionInCents;
                            break;
                        case (byte)'F': // Forfeiture
                            yearForfeitures += forfeitureInCents;
                            break;
                        default:
                            // General contribution/earnings
                            yearContributions += contributionInCents;
                            yearDividends += earningsInCents;
                            break;
                    }
                }

                // Only include years within the requested date range
                if (year >= request.BeginningDate.Year && year <= request.EndingDate.Year)
                {
                    cumulativeContributions += yearContributions;
                    cumulativeWithdrawals += yearWithdrawals;
                    cumulativeDistributions += yearDistributions;

                    // Calculate ending balance for the year
                    // Simple formula: contributions + earnings - withdrawals - distributions - forfeitures
                    long endingBalance = cumulativeContributions + yearDividends - cumulativeWithdrawals - cumulativeDistributions - yearForfeitures;

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
