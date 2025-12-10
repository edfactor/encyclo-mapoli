using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class BreakdownGrandTotalEndpoint : ProfitSharingEndpoint<YearRequest, Results<Ok<GrandTotalsByStoreResponseDto>, NotFound, ProblemHttpResult>>
{
    private readonly IBreakdownService _breakdownService;
    private readonly ILogger<BreakdownGrandTotalEndpoint> _logger;

    public BreakdownGrandTotalEndpoint(IBreakdownService breakdownService, ILogger<BreakdownGrandTotalEndpoint> logger)
        : base(Navigation.Constants.QPAY066TA)
    {
        _breakdownService = breakdownService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/breakdown-by-store/totals");
        Summary(s =>
        {
            s.Summary = "Breakdown managers and associates totals for requested store";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<AdhocReportsGroup>();

        int cacheMinutes = (!(Env.IsTestEnvironment() || Debugger.IsAttached)) ? 15 : 1;

        Options(o => o.CacheOutput(p =>
            p.Expire(TimeSpan.FromMinutes(cacheMinutes))           // same value as Cache-Control
                .SetVaryByQuery("profitYear")));                 // vary key if needed
    }

    public override async Task<Results<Ok<GrandTotalsByStoreResponseDto>, NotFound, ProblemHttpResult>> ExecuteAsync(YearRequest req, CancellationToken ct)
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var data = await _breakdownService.GetGrandTotals(req, ct);

            // Record year-end breakdown grand total metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-breakdown-grand-totals"),
                new("endpoint", "BreakdownGrandTotalEndpoint"),
                new("report_type", "grand-totals"),
                new("profit_year", req.ProfitYear.ToString()));

            _logger.LogInformation("Year-end breakdown grand totals retrieved for year {ProfitYear} (correlation: {CorrelationId})",
                req.ProfitYear, HttpContext.TraceIdentifier);

            return Result<GrandTotalsByStoreResponseDto>.Success(data);
        });
    }
}
