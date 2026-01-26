using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DeleteDistributionEndpoint : ProfitSharingEndpoint<IdRequest, Results<Ok<bool>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<DeleteDistributionEndpoint> _logger;

    public DeleteDistributionEndpoint(IDistributionService distributionService, ILogger<DeleteDistributionEndpoint> logger)
        : base(Navigation.Constants.Distributions)
    {
        _distributionService = distributionService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("{id}");
        Group<DistributionGroup>();
        Policies(Security.Policy.CanManageDistributions); // Override group policy - delete requires manage permission
        Summary(s =>
        {
            s.Summary = "Delete a profit-sharing distribution";
            s.Description = "Removes a profit-sharing distribution from the system";
            s.Response<string>(204, "Distribution deleted successfully");
            s.Response<ProblemDetails>(404, "Distribution not found");
            s.Response<ProblemDetails>(400, "Invalid request");
        });
    }

    protected override async Task<Results<Ok<bool>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(
        IdRequest req,
        CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _distributionService.DeleteDistributionAsync(req.Id, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "distribution-delete"),
                new("endpoint", "DeleteDistributionEndpoint"));

            if (result.IsSuccess)
            {
                EndpointTelemetry.RecordCountsProcessed.Record(1,
                    new("record_type", "distribution-deleted"),
                    new("endpoint", "DeleteDistributionEndpoint"));

                _logger.LogInformation("Distribution deleted for ID: {Id} (correlation: {CorrelationId})",
                    req.Id, HttpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning("Distribution deletion failed for ID: {Id} - {Error} (correlation: {CorrelationId})",
                    req.Id, result.Error, HttpContext.TraceIdentifier);
            }

            var httpResult = result.ToHttpResultWithValidation(Error.DistributionNotFound);
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
