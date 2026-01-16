using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class BreakdownGrandTotalEndpoint : ProfitSharingEndpoint<YearRequest, Results<Ok<GrandTotalsByStoreResponseDto>, NotFound, ProblemHttpResult>>
{
    private readonly IBreakdownService _breakdownService;

    public BreakdownGrandTotalEndpoint(IBreakdownService breakdownService)
        : base(Navigation.Constants.QPAY066TA)
    {
        _breakdownService = breakdownService;
    }

    public override void Configure()
    {
        Get("/stores/breakdown/totals");
        Summary(s =>
        {
            s.Summary = "Breakdown managers and associates totals for all stores";
            s.Description = "Retrieves grand total breakdown data for managers and associates across all stores. Results are cached for performance.";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<AdhocReportsGroup>();
    }

    protected override async Task<Results<Ok<GrandTotalsByStoreResponseDto>, NotFound, ProblemHttpResult>> HandleRequestAsync(YearRequest req, CancellationToken ct)
    {
        var data = await _breakdownService.GetGrandTotals(req, ct);
        return Result<GrandTotalsByStoreResponseDto>.Success(data).ToHttpResult();
    }
}
