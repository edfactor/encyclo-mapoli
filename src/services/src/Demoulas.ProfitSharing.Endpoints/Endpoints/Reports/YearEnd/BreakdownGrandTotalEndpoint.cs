using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.Util.Extensions;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class BreakdownGrandTotalEndpoint : ProfitSharingEndpoint<YearRequest, GrandTotalsByStoreResponseDto>
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

    public override Task<GrandTotalsByStoreResponseDto> ExecuteAsync(YearRequest req, CancellationToken ct)
    {
        return _breakdownService.GetGrandTotals(req, ct);
    }
}
