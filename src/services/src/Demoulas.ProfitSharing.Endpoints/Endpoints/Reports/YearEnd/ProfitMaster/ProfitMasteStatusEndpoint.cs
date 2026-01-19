using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitMaster;

public class ProfitMasterStatusEndpoint : ProfitSharingEndpoint<ProfitYearRequest, Results<Ok<ProfitMasterUpdateResponse>, NoContent, ProblemHttpResult>>
{
    private readonly IProfitMasterService _profitMasterService;
    private readonly ILogger<ProfitMasterStatusEndpoint> _logger;

    public ProfitMasterStatusEndpoint(IProfitMasterService profitMasterService, ILogger<ProfitMasterStatusEndpoint> logger)
        : base(Navigation.Constants.MasterUpdate)
    {
        _profitMasterService = profitMasterService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("profit-master-status");
        Summary(s =>
        {
            s.Summary = "Shows a summary of the current profit share update status";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object> { { 200, ProfitMasterUpdateResponse.Example() } };
        });
        Group<YearEndGroup>();
    }

    protected override async Task<Results<Ok<ProfitMasterUpdateResponse>, NoContent, ProblemHttpResult>> HandleRequestAsync(ProfitYearRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var response = await _profitMasterService.StatusAsync(req, ct);

            // Record year-end profit master status metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-profit-master-status"),
                new("endpoint", "ProfitMasterStatusEndpoint"),
                new("report_type", "status"),
                new("profit_year", req.ProfitYear.ToString()));

            _logger.LogInformation("Year-end profit master status retrieved for year {ProfitYear} (correlation: {CorrelationId})",
                req.ProfitYear, HttpContext.TraceIdentifier);

            if (response == null)
            {
                _logger.LogInformation("No profit master status data found for year {ProfitYear} (correlation: {CorrelationId})",
                    req.ProfitYear, HttpContext.TraceIdentifier);
                return TypedResults.NoContent();
            }

            this.RecordResponseMetrics(HttpContext, _logger, response);
            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
