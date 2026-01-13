using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareReport;

public sealed class YearEndProfitSharingReportTotalsEndpoint : ProfitSharingEndpoint<BadgeNumberRequest, Results<Ok<YearEndProfitSharingReportTotals>, NotFound, ProblemHttpResult>>
{
    private readonly IProfitSharingSummaryReportService _profitSharingSummaryReportService;
    private readonly IAuditService _auditService;
    private readonly ILogger<YearEndProfitSharingReportTotalsEndpoint> _logger;
    private const string ReportName = "Yearend Profit Sharing Report Totals";

    public YearEndProfitSharingReportTotalsEndpoint(
        IProfitSharingSummaryReportService profitSharingSummaryReportService,
        IAuditService auditService,
        ILogger<YearEndProfitSharingReportTotalsEndpoint> logger)
        : base(Navigation.Constants.ProfitShareReportFinalRun)
    {
        _profitSharingSummaryReportService = profitSharingSummaryReportService;
        _auditService = auditService;
        _logger = logger;
    }
    public override void Configure()
    {
        Post("yearend-profit-sharing-report-totals");
        Summary(s =>
        {
            s.Summary = ReportName;
            s.Description = "Returns the totals for the profit sharing report (426)";
            s.ExampleRequest = new BadgeNumberRequest { ProfitYear = 2025, UseFrozenData = true };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    YearEndProfitSharingReportTotals.SampleResponse()
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override Task<Results<Ok<YearEndProfitSharingReportTotals>, NotFound, ProblemHttpResult>> ExecuteAsync(BadgeNumberRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var data = await _auditService.ArchiveCompletedReportAsync(
                ReportName,
                req.ProfitYear,
                req,
                (archiveReq, _, cancellationToken) => _profitSharingSummaryReportService.GetYearEndProfitSharingTotalsAsync(archiveReq, cancellationToken),
                ct);

            // Record year-end totals report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-profit-sharing-totals"),
                new("endpoint", "YearEndProfitSharingReportTotalsEndpoint"),
                new("profit_year", req.ProfitYear.ToString()));

            return Result<YearEndProfitSharingReportTotals>.Success(data).ToHttpResult();
        });
    }
}
