using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareReport;
public sealed class YearEndProfitSharingSummaryReportEndpoint : Endpoint<BadgeNumberRequest, YearEndProfitSharingReportSummaryResponse>
{
    private readonly IProfitSharingSummaryReportService _cleanupReportService;
    private readonly IAuditService _auditService;

    public YearEndProfitSharingSummaryReportEndpoint(IProfitSharingSummaryReportService cleanupReportService, IAuditService auditService)
    {
        _cleanupReportService = cleanupReportService;
        _auditService = auditService;
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

    public override async Task<YearEndProfitSharingReportSummaryResponse> ExecuteAsync(BadgeNumberRequest req, CancellationToken ct)
    {
        var response = await _cleanupReportService.GetYearEndProfitSharingSummaryReportAsync(req, ct);

        // Read "archive" from query string without modifying the DTO
        bool archive = HttpContext.Request.Query.TryGetValue("archive", out var archiveValue) &&
                       bool.TryParse(archiveValue, out var archiveResult) && archiveResult;

        if (archive)
        {
            var keyValues = new List<KeyValuePair<string, decimal>>();
            foreach ( var lineItem in response.LineItems)
            {
                keyValues.Add(new KeyValuePair<string, decimal>(lineItem.LineItemTitle, lineItem.NumberOfMembers));
                keyValues.Add(new KeyValuePair<string, decimal>(lineItem.LineItemTitle, lineItem.TotalWages));
                keyValues.Add(new KeyValuePair<string, decimal>(lineItem.LineItemTitle, lineItem.TotalBalance));
            }
            
            await _auditService.ArchiveCompletedReportAsync("Yearend profit sharing summary report", req.ProfitYear, req, response, keyValues, ct);
        }

        return response;
    }
}
