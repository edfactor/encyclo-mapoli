using Demoulas.Common.Api.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Builder;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class DistributionGroup : GroupBase
{
    public DistributionGroup() : base()
    {
        Configure(Route.ToLowerInvariant(), ep =>
        {
            ep.Description(x => x
                .ProducesProblemFE<ProblemDetails>()
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status401Unauthorized)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status500InternalServerError)
                .WithRequestTimeout(TimeSpan.FromMinutes(1))
                .WithTags(RouteName));

            ep.Policies(Policy.CanViewDistributions);
        });
    }
    protected override string Route => "distributions";
    protected override string RouteName => "Distributions";
}
