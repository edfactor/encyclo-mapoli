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
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class BreakdownTotalsEndpoint : ProfitSharingEndpoint<BreakdownTotalsByStoreRequest, Results<Ok<BreakdownByStoreTotals>, NotFound, ProblemHttpResult>>
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

    public override async Task<Results<Ok<BreakdownByStoreTotals>, NotFound, ProblemHttpResult>> ExecuteAsync(BreakdownTotalsByStoreRequest req, CancellationToken ct)
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
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

            // Record year-end breakdown store totals metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-breakdown-store-totals"),
                new("endpoint", "BreakdownTotalsEndpoint"),
                new("report_type", "store-totals"),
                new("store_number", req.StoreNumber.ToString()));

            _logger.LogInformation("Year-end breakdown store totals retrieved for store {StoreNumber} (correlation: {CorrelationId})",
                req.StoreNumber, HttpContext.TraceIdentifier);

            return Result<BreakdownByStoreTotals>.Success(data);
        });
    }
}
