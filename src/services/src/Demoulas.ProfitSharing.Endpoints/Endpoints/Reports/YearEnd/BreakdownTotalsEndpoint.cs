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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class BreakdownTotalsEndpoint : ProfitSharingEndpoint<BreakdownByStoreRequest, Results<Ok<BreakdownByStoreTotals>, NotFound, ProblemHttpResult>>
{
    private readonly IBreakdownService _breakdownService;
    private readonly ILogger<BreakdownTotalsEndpoint> _logger;

    public BreakdownTotalsEndpoint(IBreakdownService breakdownService, ILogger<BreakdownTotalsEndpoint> logger)
        : base(Navigation.Constants.QPAY066TA)
    {
        _breakdownService = breakdownService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/breakdown-by-store/{@storeNumber}/totals", request => new { request.StoreNumber });
        Summary(s =>
        {
            s.Summary = "Breakdown managers and associates totals for requested store";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<AdhocReportsGroup>();
    }

    public override async Task<Results<Ok<BreakdownByStoreTotals>, NotFound, ProblemHttpResult>> ExecuteAsync(BreakdownByStoreRequest req, CancellationToken ct)
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var data = await _breakdownService.GetTotalsByStore(req, ct);

            // Record year-end breakdown store totals metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-breakdown-store-totals"),
                new("endpoint", "BreakdownTotalsEndpoint"),
                new("report_type", "store-totals"),
                new("store_number", req.StoreNumber?.ToString() ?? "all"));

            _logger.LogInformation("Year-end breakdown store totals retrieved for store {StoreNumber} (correlation: {CorrelationId})",
                req.StoreNumber, HttpContext.TraceIdentifier);

            return Result<BreakdownByStoreTotals>.Success(data);
        });
    }
}
