using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
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
        var result = await _distributionService.DeleteDistributionAsync(req.Id, ct);
        _logger.LogInformation("Distribution delete requested for id {DistributionId}", req.Id);
        return result.ToHttpResultWithValidation(Error.DistributionNotFound);
    }
}
