using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public class NegativeEtvaReportService : INegativeEtvaReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly ICalendarService _calendarService;
    private readonly ILogger<NegativeEtvaReportService> _logger;

    public NegativeEtvaReportService(IProfitSharingDataContextFactory dataContextFactory,
        ILoggerFactory loggerFactory,
        IDemographicReaderService demographicReaderService,
        ICalendarService calendarService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _calendarService = calendarService;
        _logger = loggerFactory.CreateLogger<NegativeEtvaReportService>();
    }

    public async Task<ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>> GetNegativeETVAForSsNsOnPayProfitResponseAsync(ProfitYearRequest req, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("Request NEGATIVE ETVA FOR SSNs ON PAYPROFIT"))
        {
            var data = await _dataContextFactory.UseReadOnlyContext(async c =>
            {
                var demographics = await _demographicReaderService.BuildDemographicQuery(c);
                
                var ssnUnion = demographics.Select(d => d.Ssn)
                    .Union(c.Beneficiaries.Include(b => b.Contact).Select(b => b.Contact!.Ssn));

                return await c.PayProfits
                    .Include(p => p.Demographic)
                    .Where(p => p.ProfitYear == req.ProfitYear
                                && ssnUnion.Contains(p.Demographic!.Ssn)
                                && p.Etva < 0)
                    .Select(p => new
                    {
                        p.Demographic!.BadgeNumber,
                        p.Demographic.Ssn,
                        EtvaValue = p.Etva,
                        IsExecutive = p.Demographic.PayFrequencyId == PayFrequency.Constants.Monthly,
                    })
                    .OrderBy(p => p.BadgeNumber)
                    .ToPaginationResultsAsync(req, cancellationToken);
            });

            var results = new Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<NegativeEtvaForSsNsOnPayProfitResponse>
            {
                Results = data.Results.Select(r => new NegativeEtvaForSsNsOnPayProfitResponse
                {
                    BadgeNumber = r.BadgeNumber,
                    Ssn = r.Ssn.MaskSsn(),
                    EtvaValue = r.EtvaValue,
                    IsExecutive = r.IsExecutive
                }),
                Total = data.Total,
            };

            _logger.LogInformation("Returned {Results} records", results.Results.Count());

            var cal = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);

            return new ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>
            {
                ReportName = "NEGATIVE ETVA FOR SSNs ON PAYPROFIT",
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = cal.FiscalBeginDate,
                EndDate = cal.FiscalEndDate,
                Response = results
            };
        }
    }
}
