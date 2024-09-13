using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class LookupGroup : Group
{
    private const string Route = "lookup";
    private const string RouteName = "Lookup";
    public LookupGroup()
    {
        Configure(Route.ToLowerInvariant(), ep => //admin is the route prefix for the top level group
        {
            ep.Description(x => x
                .Produces((int)HttpStatusCode.Unauthorized)
                .Produces((int)HttpStatusCode.Forbidden)
                .Produces((int)HttpStatusCode.RequestTimeout)
                .Produces((int)HttpStatusCode.TooManyRequests)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>()
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>((int)HttpStatusCode.NotAcceptable)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>((int)HttpStatusCode.NotFound)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>((int)HttpStatusCode.UnprocessableEntity)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>((int)HttpStatusCode.MethodNotAllowed)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>((int)HttpStatusCode.InternalServerError)
                .WithTags(RouteName));
        });
    }
}
