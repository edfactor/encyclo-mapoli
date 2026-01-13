using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Navigations;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Navigations;

public class UpdateNavigationStatusEndpoint : ProfitSharingEndpoint<UpdateNavigationRequestDto, Results<Ok<UpdateNavigationStatusResponseDto>, NotFound, ProblemHttpResult>>
{

    private readonly INavigationService _navigationService;
    private readonly ILogger<UpdateNavigationStatusEndpoint> _logger;

    public UpdateNavigationStatusEndpoint(INavigationService navigationService, ILogger<UpdateNavigationStatusEndpoint> logger) : base(Navigation.Constants.Unknown)
    {
        _navigationService = navigationService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("");
        Summary(m =>
        {
            m.Summary = "Update navigation Status";
            m.Description = "Get the navigationId and statusId and update navigation status.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new UpdateNavigationStatusResponseDto() } };
        });
        Group<NavigationGroup>();
    }

    public override Task<Results<Ok<UpdateNavigationStatusResponseDto>, NotFound, ProblemHttpResult>> ExecuteAsync(UpdateNavigationRequestDto req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var isSuccessful = await _navigationService.UpdateNavigation(req.NavigationId, req.StatusId, cancellationToken: ct);
            var response = new UpdateNavigationStatusResponseDto { IsSuccessful = isSuccessful };

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "update"),
                new KeyValuePair<string, object?>("endpoint.category", "navigation"));

            // Log navigation update
            _logger.LogInformation("Navigation status updated: NavigationId={NavigationId}, StatusId={StatusId}, Success={IsSuccessful} (correlation: {CorrelationId})",
                req.NavigationId, req.StatusId, isSuccessful, HttpContext.TraceIdentifier);

            return Result<UpdateNavigationStatusResponseDto>.Success(response).ToHttpResult();
        });
    }

}
