using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class BreakdownGrandTotalEndpoint : Endpoint<YearRequest, GrandTotalsByStoreResponseDto>
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

        if (!(Env.IsTestEnvironment() || Debugger.IsAttached))
        {
            // Specify caching duration and store it in metadata
            TimeSpan cacheDuration = TimeSpan.FromMinutes(15);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override Task<GrandTotalsByStoreResponseDto> ExecuteAsync(YearRequest req, CancellationToken ct)
    {
        return _breakdownService.GetGrandTotals(req, ct);
    }
}
