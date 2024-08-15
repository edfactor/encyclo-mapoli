using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class JobsGroup : Group
{
    private const string Route = "jobs";
    private const string RouteName = "Jobs";
    public JobsGroup()
    {
        Configure(Route.ToLowerInvariant(), ep => //admin is the route prefix for the top level group
        {
            ep.Description(x => x
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>()
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(500)
                .WithTags(RouteName));
        });
    }
}
