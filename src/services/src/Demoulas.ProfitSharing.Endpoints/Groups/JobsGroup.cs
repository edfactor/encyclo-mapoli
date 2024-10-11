using System.Net;
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
                .Produces((int)HttpStatusCode.Unauthorized)
                .Produces((int)HttpStatusCode.Forbidden)
                .Produces((int)HttpStatusCode.NotFound)
                .Produces((int)HttpStatusCode.TooManyRequests)
                .Produces((int)HttpStatusCode.MethodNotAllowed)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>()
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>((int)HttpStatusCode.InternalServerError)
                .WithTags(RouteName));
        });
    }
}
