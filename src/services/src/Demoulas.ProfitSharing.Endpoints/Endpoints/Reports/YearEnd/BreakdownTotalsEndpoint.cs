using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class BreakdownTotalsEndpoint : ProfitSharingEndpoint<BreakdownByStoreRequest, BreakdownByStoreTotals>
{
    private readonly IBreakdownService _breakdownService;

    public BreakdownTotalsEndpoint(IBreakdownService breakdownService)
        : base(Navigation.Constants.QPAY066TA)
    {
        _breakdownService = breakdownService;
    }

    public override void Configure()
    {
        Get("/breakdown-by-store/{@storeNumber}/totals", request => new {request.StoreNumber});
        Summary(s =>
        {
            s.Summary = "Breakdown managers and associates totals for requested store";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override Task<BreakdownByStoreTotals> ExecuteAsync(BreakdownByStoreRequest req, CancellationToken ct)
    {
        return _breakdownService.GetTotalsByStore(req, ct);
    }
}
