using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class UpdateDistributionEndpoint : ProfitSharingEndpoint<UpdateDistributionRequest, Results<Ok<CreateOrUpdateDistributionResponse>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<UpdateDistributionEndpoint> _logger;

    public UpdateDistributionEndpoint(IDistributionService distributionService, ILogger<UpdateDistributionEndpoint> logger) : base(Navigation.Constants.Distributions)
    {
        _distributionService = distributionService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/");
        Group<DistributionGroup>();
        Policies(Security.Policy.CanManageDistributions); // Override group policy - update requires manage permission
        Summary(s =>
        {
            s.Summary = "Updates an existing profit sharing distribution in the current profit year.";
            s.Description = "Updates an existing profit sharing distribution record for the current profit year. ";
            s.ExampleRequest = UpdateDistributionRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    CreateOrUpdateDistributionResponse.ResponseExample()
                }
            };
        });
    }

    public override async Task<Results<Ok<CreateOrUpdateDistributionResponse>, NotFound, BadRequest, ProblemHttpResult>> ExecuteAsync(UpdateDistributionRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _distributionService.UpdateDistribution(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "distribution-update"),
                new("endpoint", "UpdateDistributionEndpoint"));

            if (result.IsSuccess)
            {
                EndpointTelemetry.RecordCountsProcessed.Record(1,
                    new("record_type", "distribution-updated"),
                    new("endpoint", "UpdateDistributionEndpoint"));

                _logger.LogInformation("Distribution updated for ID: {Id}, Badge: {BadgeNumber}, Gross Amount: {GrossAmount} (correlation: {CorrelationId})",
                    req.Id, result.Value?.BadgeNumber, result.Value?.GrossAmount, HttpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning("Distribution update failed for ID: {Id} - {Error} (correlation: {CorrelationId})",
                    req.Id, result.Error, HttpContext.TraceIdentifier);
            }

            var httpResult = result.ToHttpResultWithValidation(Error.DistributionNotFound, Error.BadgeNumberNotFound);
            this.RecordResponseMetrics(HttpContext, _logger, httpResult);

            return httpResult;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
