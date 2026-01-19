using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareReport;

public sealed class YearEndProfitSharingReportTotalsEndpoint : ProfitSharingEndpoint<BadgeNumberRequest, Results<Ok<YearEndProfitSharingReportTotals>, NotFound, ProblemHttpResult>>
{
    private readonly IProfitSharingSummaryReportService _profitSharingSummaryReportService;
    private readonly IProfitSharingAuditService _profitSharingAuditService;
    private const string ReportName = "Yearend Profit Sharing Report Totals";

    public YearEndProfitSharingReportTotalsEndpoint(
        IProfitSharingSummaryReportService profitSharingSummaryReportService,
        IProfitSharingAuditService profitSharingAuditService)
        : base(Navigation.Constants.ProfitShareReportFinalRun)
    {
        _profitSharingSummaryReportService = profitSharingSummaryReportService;
        _profitSharingAuditService = profitSharingAuditService;
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

    protected override async Task<Results<Ok<YearEndProfitSharingReportTotals>, NotFound, ProblemHttpResult>> HandleRequestAsync(BadgeNumberRequest req, CancellationToken ct)
    {
        var data = await _profitSharingAuditService.ArchiveCompletedReportAsync(
            ReportName,
            req.ProfitYear,
            req,
            (archiveReq, _, cancellationToken) => _profitSharingSummaryReportService.GetYearEndProfitSharingTotalsAsync(archiveReq, cancellationToken),
            ct);

        return Result<YearEndProfitSharingReportTotals>.Success(data).ToHttpResult();
    }
}
