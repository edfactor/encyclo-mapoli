using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareReport;
public sealed class YearEndProfitSharingReportTotalsEndpoint: Endpoint<BadgeNumberRequest, YearEndProfitSharingReportTotals>
{
    private readonly IProfitSharingSummaryReportService _profitSharingSummaryReportService;
    private readonly IAuditService _auditService;
    private const string ReportName = "Yearend Profit Sharing Report Totals";

    public YearEndProfitSharingReportTotalsEndpoint(
        IProfitSharingSummaryReportService profitSharingSummaryReportService,
        IAuditService auditService)
    {
        _profitSharingSummaryReportService = profitSharingSummaryReportService;
        _auditService = auditService;
    }
    public override void Configure()
    {
        Post("yearend-profit-sharing-report-totals");
        Summary(s =>
        {
            s.Summary = ReportName;
            s.Description = "Returns the totals for the profit sharing report (426)";
            s.ExampleRequest = new BadgeNumberRequest { ProfitYear = 2025, UseFrozenData = true};
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

    public override Task<YearEndProfitSharingReportTotals> ExecuteAsync(BadgeNumberRequest req, CancellationToken ct)
    {
        return _auditService.ArchiveCompletedReportAsync(
            ReportName,
            req.ProfitYear,
            req,
            (archiveReq, _, cancellationToken) => _profitSharingSummaryReportService.GetYearEndProfitSharingTotalsAsync(archiveReq, cancellationToken),
            ct);
    }
}
