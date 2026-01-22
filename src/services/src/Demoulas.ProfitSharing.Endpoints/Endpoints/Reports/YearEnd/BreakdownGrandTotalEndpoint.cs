using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class BreakdownGrandTotalEndpoint : ProfitSharingEndpoint<GrandTotalsByStoreRequest, Results<Ok<GrandTotalsByStoreResponseDto>, NotFound, ProblemHttpResult>>
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
            s.Description = "Retrieves grand total breakdown data for managers and associates across all stores. Results are cached for performance. Supports filtering for participants under 21 years old.";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<AdhocReportsGroup>();
    }

    protected override Task<Results<Ok<GrandTotalsByStoreResponseDto>, NotFound, ProblemHttpResult>> HandleRequestAsync(GrandTotalsByStoreRequest req, CancellationToken ct)
    {
        return _breakdownService.GetGrandTotals(req, ct)
            .ContinueWith(task => Result<GrandTotalsByStoreResponseDto>.Success(task.Result).ToHttpResult(), ct, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
    }
}
