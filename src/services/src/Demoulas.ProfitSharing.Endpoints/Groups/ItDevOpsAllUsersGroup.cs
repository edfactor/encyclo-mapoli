using Demoulas.Common.Api.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class ItDevOpsAllUsersGroup : GroupBase
{
    protected override string Route => "itdevops";
    protected override string RouteName => "ItDevOps";

    public ItDevOpsAllUsersGroup()
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
        });
    }
}
