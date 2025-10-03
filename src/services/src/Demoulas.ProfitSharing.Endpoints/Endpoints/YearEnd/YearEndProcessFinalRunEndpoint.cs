using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.YearEnd;

public class YearEndProcessFinalRunEndpoint : ProfitSharingRequestEndpoint<YearRequestWithRebuild>
{
    private readonly IYearEndService _yearEndService;
    private readonly ILogger<YearEndProcessFinalRunEndpoint> _logger;

    public YearEndProcessFinalRunEndpoint(IYearEndService yearEndService, ILogger<YearEndProcessFinalRunEndpoint> logger)
        : base(Navigation.Constants.ProfitShareReportFinalRun)
    {
        _yearEndService = yearEndService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("final");
        Summary(s => { s.Summary = "Updates data in prior to final run of the Profit Sharing report"; });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(YearRequestWithRebuild req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // Record request metrics
            this.RecordRequestMetrics(HttpContext, _logger, req);

            await _yearEndService.RunFinalYearEndUpdates(req.ProfitYear, req.Rebuild, ct);

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "final-year-end-updates"),
                new KeyValuePair<string, object?>("endpoint.category", "year-end"));

            // Log final run processing (this is a significant operation)
            _logger.LogInformation("Year-end final run processing completed for profit year {ProfitYear}, rebuild: {Rebuild} (correlation: {CorrelationId})",
                req.ProfitYear, req.Rebuild, HttpContext.TraceIdentifier);

            await Send.NoContentAsync(ct);

            // Record successful response
            this.RecordResponseMetrics(HttpContext, _logger, new { Success = true }, isSuccess: true);
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
