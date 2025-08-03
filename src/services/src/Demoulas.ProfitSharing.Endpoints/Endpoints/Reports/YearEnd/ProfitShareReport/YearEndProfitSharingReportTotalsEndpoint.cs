using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareReport;
public sealed class YearEndProfitSharingReportTotalsEndpoint: Endpoint<BadgeNumberRequest, YearEndProfitSharingReportTotals>
{
    private readonly IProfitSharingSummaryReportService _profitSharingSummaryReportService;

    public YearEndProfitSharingReportTotalsEndpoint(
        IProfitSharingSummaryReportService profitSharingSummaryReportService
        ) {
        _profitSharingSummaryReportService = profitSharingSummaryReportService;
    }
    public override void Configure()
    {
        Post("yearend-profit-sharing-report-totals");
        Summary(s =>
        {
            s.Summary = "Yearend profit sharing report totals";
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
        return _profitSharingSummaryReportService.GetYearEndProfitSharingTotalsAsync(req, ct);
    }
}
