using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ExecutiveHoursAndDollars;

public class SetExecutiveHoursAndDollarsEndpoint : ProfitSharingRequestEndpoint<SetExecutiveHoursAndDollarsRequest>
{
    private readonly IExecutiveHoursAndDollarsService _executiveHoursAndDollarsService;
    private readonly ILogger<SetExecutiveHoursAndDollarsEndpoint> _logger;

    public SetExecutiveHoursAndDollarsEndpoint(IExecutiveHoursAndDollarsService executiveHoursAndDollarsService, ILogger<SetExecutiveHoursAndDollarsEndpoint> logger) : base(Navigation.Constants.ManageExecutiveHours)
    {
        _executiveHoursAndDollarsService = executiveHoursAndDollarsService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("executive-hours-and-dollars");
        Summary(s =>
        {
            s.Summary = "Executive Hours and Dollars Endpoint";
            s.Description =
                "This endpoint allows the executive hours and dollars to be set.";

            s.ExampleRequest = SetExecutiveHoursAndDollarsRequest.RequestExample();

            s.Responses[204] = "Success, No Content";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });

        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(SetExecutiveHoursAndDollarsRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            await _executiveHoursAndDollarsService.SetExecutiveHoursAndDollarsAsync(req.ProfitYear, req.ExecutiveHoursAndDollars, ct);

            // Record year-end executive hours and dollars set metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-set-executive-hours-dollars"),
                new("endpoint", "SetExecutiveHoursAndDollarsEndpoint"),
                new("report_type", "executive-hours-dollars"),
                new("operation_type", "set"),
                new("profit_year", req.ProfitYear.ToString()));

            var recordCount = req.ExecutiveHoursAndDollars?.Count ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                new("record_type", "executive-hours-dollars-set"),
                new("endpoint", "SetExecutiveHoursAndDollarsEndpoint"));

            _logger.LogInformation("Year-end executive hours and dollars set for year {ProfitYear}, processed {Count} records (correlation: {CorrelationId})",
                req.ProfitYear, recordCount, HttpContext.TraceIdentifier);
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
