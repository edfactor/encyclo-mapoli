using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareReport;
public sealed class YearEndProfitSharingSummaryReportEndpoint : Endpoint<ProfitYearRequest, YearEndProfitSharingReportSummaryResponse>
{
    private readonly IProfitSharingSummaryReportService _cleanupReportService;

    public YearEndProfitSharingSummaryReportEndpoint(IProfitSharingSummaryReportService cleanupReportService)
    {
        _cleanupReportService = cleanupReportService;
    }

    public override void Configure()
    {
        Get("yearend-profit-sharing-summary-report");
        Summary(s =>
        {
            s.Summary = "Yearend profit sharing summary report";
            s.Description = "Returns a breakdown of member counts/sum by various descriminators";
            s.ExampleRequest = new ProfitYearRequest { ProfitYear = 2025 };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    YearEndProfitSharingReportSummaryResponse.SampleResponse()
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override Task<YearEndProfitSharingReportSummaryResponse> ExecuteAsync(ProfitYearRequest req, CancellationToken ct)
    {
        return _cleanupReportService.GetYearEndProfitSharingSummaryReportAsync(req, ct);
    }
}
