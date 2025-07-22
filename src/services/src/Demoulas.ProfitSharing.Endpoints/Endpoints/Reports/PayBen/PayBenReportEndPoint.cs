using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using FastEndpoints;
using Demoulas.ProfitSharing.Common.Contracts.Request.PayBen;
using Demoulas.ProfitSharing.Common.Contracts.Response.PayBen;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.PayBen;

public class PayBenReportEndpoint : Endpoint<PayBenReportRequest,
    PaginatedResponseDto<PayBenReportResponse>>
{
    private readonly IPayBenReportService _reportService;

    public PayBenReportEndpoint(IPayBenReportService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("payben-report");
        Summary(s =>
        {
            s.Summary = "PayBen Report";
            s.Description = "Returns a report of beneficiary and their percentage";
            s.ExampleRequest = new PayBenReportRequest();
            s.ResponseExamples = new Dictionary<int, object> { { 200, new PaginatedResponseDto<PayBenReportResponse>() } };
        });
        Group<YearEndGroup>();
    }


    public override  Task<PaginatedResponseDto<PayBenReportResponse>> ExecuteAsync(PayBenReportRequest req, CancellationToken ct)
    {
        return _reportService.GetPayBenReport(req, ct);
    }
}
