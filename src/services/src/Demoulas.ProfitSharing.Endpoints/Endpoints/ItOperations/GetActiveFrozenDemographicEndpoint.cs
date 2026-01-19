using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

public class GetActiveFrozenDemographicEndpoint : ProfitSharingResponseEndpoint<FrozenStateResponse>
{
    private readonly IFrozenService _frozenService;
    private readonly ILogger<GetActiveFrozenDemographicEndpoint> _logger;

    public GetActiveFrozenDemographicEndpoint(IFrozenService frozenService, ILogger<GetActiveFrozenDemographicEndpoint> logger) : base(Navigation.Constants.DemographicFreeze)
    {
        _frozenService = frozenService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("frozen/active");
        Summary(s =>
        {
            s.Summary = "Gets frozen demographic meta data";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new FrozenStateResponse { Id = 2, ProfitYear = Convert.ToInt16(DateTime.Now.Year), IsActive = true }
                }
            };
        });
        Group<ItDevOpsAllUsersGroup>();
    }

    protected override async Task<FrozenStateResponse> HandleRequestAsync(CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, new { });

            var response = await _frozenService.GetActiveFrozenDemographic(ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "active-frozen-demographic-query"),
                new("endpoint", "GetActiveFrozenDemographicEndpoint"));

            EndpointTelemetry.RecordCountsProcessed.Record(response == null ? 0 : 1,
                new("record_type", "active-frozen-demographic"),
                new("endpoint", "GetActiveFrozenDemographicEndpoint"));

            _logger.LogInformation("Active frozen demographic query completed, found: {HasActiveFrozen}, ProfitYear: {ProfitYear} (correlation: {CorrelationId})",
                response != null, response?.ProfitYear, HttpContext.TraceIdentifier);

            var safeResponse = response ?? new FrozenStateResponse { Id = 0 };
            this.RecordResponseMetrics(HttpContext, _logger, safeResponse);

            return safeResponse;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
