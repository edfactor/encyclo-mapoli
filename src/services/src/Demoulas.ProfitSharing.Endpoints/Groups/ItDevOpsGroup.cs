using Demoulas.Common.Api.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class ItDevOpsGroup : GroupBase
{
    protected override string Route => "itdevops";
    protected override string RouteName => "ItDevOps";

    public ItDevOpsGroup()
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
            // Freeze endpoints are restricted to IT DevOps via the CanFreezeDemographics policy.
            ep.Policies(Policy.CanFreezeDemographics);
        });
    }
}
