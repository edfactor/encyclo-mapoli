using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class BreakdownGrandTotalEndpoint : Endpoint<YearRequest, BreakdownByStoreTotals>
{
    private readonly IBreakdownService _breakdownService;

    public BreakdownGrandTotalEndpoint(IBreakdownService breakdownService)
    {
        _breakdownService = breakdownService;
    }

    public override void Configure()
    {
        Get("/breakdown-by-store/totals");
        Summary(s =>
        {
            s.Summary = "Breakdown managers and associates totals for requested store";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override Task<BreakdownByStoreTotals> ExecuteAsync(YearRequest req, CancellationToken ct)
    {
        return _breakdownService.GetGrandTotals(req, ct);
    }
}
