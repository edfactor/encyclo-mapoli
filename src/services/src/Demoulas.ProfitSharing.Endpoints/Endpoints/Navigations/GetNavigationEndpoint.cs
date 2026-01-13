using Demoulas.ProfitSharing.Common.Contracts.Request.Navigations;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Navigations;

public class GetNavigationEndpoint : ProfitSharingEndpoint<NavigationRequestDto, NavigationResponseDto>
{

    private readonly INavigationService _navigationService;
    private readonly ILogger<GetNavigationEndpoint> _logger;

    public GetNavigationEndpoint(INavigationService navigationService, ILogger<GetNavigationEndpoint> logger)
        : base(Navigation.Constants.Unknown)
    {
        _navigationService = navigationService;
        _logger = logger;
    }

    public override void Configure()
    {

        Get("");
        Summary(m =>
        {
            m.Summary = "Get all navigation";
            m.Description = "Fetch List of navigation object.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new NavigationResponseDto() } };
        });
        Group<NavigationGroup>();
    }

    public override Task<NavigationResponseDto> ExecuteAsync(NavigationRequestDto req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var response = await _navigationService.GetNavigation(cancellationToken: ct);

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "get"),
                new KeyValuePair<string, object?>("endpoint.category", "navigation"));

            // Record navigation count for monitoring
            var navigationCount = response.Navigation?.Count ?? 0;
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.RecordCountsProcessed.Record(navigationCount,
                new KeyValuePair<string, object?>("endpoint.name", nameof(GetNavigationEndpoint)),
                new KeyValuePair<string, object?>("operation", "get"));

            // Log navigation retrieval
            _logger.LogInformation("Navigation list retrieved: {NavigationCount} items (correlation: {CorrelationId})",
                navigationCount, HttpContext.TraceIdentifier);

            return response;
        });
    }

}
