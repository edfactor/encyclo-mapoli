using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class DeleteDistributionEndpoint : ProfitSharingEndpoint<IdRequest, Results<Ok<bool>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<DeleteDistributionEndpoint> _logger;

    public DeleteDistributionEndpoint(IDistributionService distributionService, ILogger<DeleteDistributionEndpoint> logger) : base(Navigation.Constants.Distributions)
    {
        _distributionService = distributionService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("{Id}");
        Group<DistributionGroup>();
        Policies(Policy.CanManageDistributions); // Override group policy - delete requires manage permission
        Summary(s =>
        {
            s.Summary = "Delete a profit-sharing distribution";
            s.Description = "Removes a profit-sharing distribution from the system";
            s.Response<string>(204, "Distribution deleted successfully");
            s.Response<ProblemDetails>(404, "Distribution not found");
            s.Response<ProblemDetails>(400, "Invalid request");
        });
    }

    public override Task<Results<Ok<bool>, NotFound, BadRequest, ProblemHttpResult>> ExecuteAsync(IdRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _distributionService.DeleteDistribution(req.Id, ct);

            // Add business metrics for distribution deletion
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "distribution-deletion"),
                new("endpoint", nameof(DeleteDistributionEndpoint)));

            return result.ToHttpResultWithValidation(Error.DistributionNotFound);
        });
    }
}
