using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.PayBen;
using Demoulas.ProfitSharing.Common.Contracts.Response.PayBen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.PayBen;

public class PayBenReportEndpoint : ProfitSharingEndpoint<PayBenReportRequest,
    PaginatedResponseDto<PayBenReportResponse>>
{
    private readonly IPayBenReportService _reportService;

    public PayBenReportEndpoint(IPayBenReportService reportService)
        : base(Navigation.Constants.PayBenReport)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
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

    protected override async Task<PaginatedResponseDto<PayBenReportResponse>> HandleRequestAsync(PayBenReportRequest req, CancellationToken ct)
    {
        var result = await _reportService.GetPayBenReport(req, ct);
        return result ?? new PaginatedResponseDto<PayBenReportResponse>();
    }
}
