using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class BreakdownTotalsEndpoint : ProfitSharingEndpoint<BreakdownByStoreRequest, Results<Ok<BreakdownByStoreTotals>, NotFound, ProblemHttpResult>>
{
    private readonly IBreakdownService _breakdownService;

    public BreakdownTotalsEndpoint(IBreakdownService breakdownService)
        : base(Navigation.Constants.QPAY066TA)
    {
        _breakdownService = breakdownService;
    }

    public override void Configure()
    {
        Get("/breakdown-by-store/{@storeNumber}/totals", request => new { request.StoreNumber });
        Summary(s =>
        {
            s.Summary = "Breakdown managers and associates totals for requested store";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override async Task<Results<Ok<BreakdownByStoreTotals>, NotFound, ProblemHttpResult>> ExecuteAsync(BreakdownByStoreRequest req, CancellationToken ct)
    {
        try
        {
            var data = await _breakdownService.GetTotalsByStore(req, ct);
            return Result<BreakdownByStoreTotals>.Success(data).ToHttpResult();
        }
        catch (Exception ex)
        {
            return Result<BreakdownByStoreTotals>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
