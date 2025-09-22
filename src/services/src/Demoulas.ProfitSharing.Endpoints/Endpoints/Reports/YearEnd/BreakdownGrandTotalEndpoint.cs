using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.Util.Extensions;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;

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
        Get("/breakdown-by-store/totals");
        Summary(s =>
        {
            s.Summary = "Breakdown managers and associates totals for requested store";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();

        int cacheMinutes = (!(Env.IsTestEnvironment() || Debugger.IsAttached)) ? 15 : 1;

        Options(o => o.CacheOutput(p =>
            p.Expire(TimeSpan.FromMinutes(cacheMinutes))           // same value as Cache-Control
                .SetVaryByQuery("profitYear")));                 // vary key if needed
    }

    public override async Task<Results<Ok<GrandTotalsByStoreResponseDto>, NotFound, ProblemHttpResult>> ExecuteAsync(YearRequest req, CancellationToken ct)
    {
        try
        {
            var data = await _breakdownService.GetGrandTotals(req, ct);
            return Result<GrandTotalsByStoreResponseDto>.Success(data).ToHttpResult();
        }
        catch (Exception ex)
        {
            return Result<GrandTotalsByStoreResponseDto>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
