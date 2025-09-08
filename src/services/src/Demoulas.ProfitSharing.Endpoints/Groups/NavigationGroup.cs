using Demoulas.Common.Api.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class NavigationGroup : GroupBase
{
    protected override string Route => "navigation";
    protected override string RouteName => "Navigation";

    public NavigationGroup()
    {
        Configure(Route.ToLowerInvariant(), ep =>
        {
            ep.Description(x => x
                .ProducesProblemFE<ProblemDetails>()
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status401Unauthorized)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status500InternalServerError)
                .WithTags(RouteName));
            // Authentication only (no specific authorization policy).
        });
    }
}
