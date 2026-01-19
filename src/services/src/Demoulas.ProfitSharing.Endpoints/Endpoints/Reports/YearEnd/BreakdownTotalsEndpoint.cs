using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class BreakdownTotalsEndpoint : ProfitSharingEndpoint<BreakdownTotalsByStoreRequest, Results<Ok<BreakdownByStoreTotals>, NotFound, ProblemHttpResult>>
{
    private readonly IBreakdownService _breakdownService;

    public BreakdownTotalsEndpoint(IBreakdownService breakdownService)
        : base(Navigation.Constants.QPAY066TA)
    {
        _breakdownService = breakdownService;
    }

    public override void Configure()
    {
        Get("/stores/{StoreNumber:int}/breakdown/totals");
        Summary(s =>
        {
            s.Summary = "Breakdown managers and associates totals for requested store";
            s.Description = "Retrieves total breakdown data for managers and associates at a specific store. Requires QPAY066TA authorization.";
            s.ExampleRequest = BreakdownTotalsByStoreRequest.RequestExample();
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<AdhocReportsGroup>();
    }

    protected override async Task<Results<Ok<BreakdownByStoreTotals>, NotFound, ProblemHttpResult>> HandleRequestAsync(BreakdownTotalsByStoreRequest req, CancellationToken ct)
    {
        var serviceRequest = new BreakdownByStoreRequest
        {
            ProfitYear = req.ProfitYear,
            StoreManagement = req.StoreManagement,
            StoreNumber = req.StoreNumber,
            BadgeNumber = req.BadgeNumber,
            EmployeeName = req.EmployeeName
        };

        var data = await _breakdownService.GetTotalsByStore(serviceRequest, ct);
        return Result<BreakdownByStoreTotals>.Success(data).ToHttpResult();
    }
}
