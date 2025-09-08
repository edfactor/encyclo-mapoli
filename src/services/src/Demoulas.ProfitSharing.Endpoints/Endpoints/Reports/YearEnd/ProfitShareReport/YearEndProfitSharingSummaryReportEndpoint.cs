using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareReport;
public sealed class YearEndProfitSharingSummaryReportEndpoint : ProfitSharingEndpoint<BadgeNumberRequest, YearEndProfitSharingReportSummaryResponse>
{
    private readonly IProfitSharingSummaryReportService _cleanupReportService;

    public YearEndProfitSharingSummaryReportEndpoint(IProfitSharingSummaryReportService cleanupReportService)
        : base(Navigation.Constants.ProfitShareReportFinalRun)
    {
        _cleanupReportService = cleanupReportService;
    }

    public override void Configure()
    {
        Post("yearend-profit-sharing-summary-report");
        Summary(s =>
        {
            s.Summary = "Yearend profit sharing summary report";
            s.Description = "Returns a breakdown of member counts/sum by various descriminators";
            s.ExampleRequest = new BadgeNumberRequest { ProfitYear = 2025, BadgeNumber = 723456, UseFrozenData = false};
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

    public override Task<YearEndProfitSharingReportSummaryResponse> ExecuteAsync(BadgeNumberRequest req, CancellationToken ct)
    {
     return _cleanupReportService.GetYearEndProfitSharingSummaryReportAsync(req, ct);
    }
}
