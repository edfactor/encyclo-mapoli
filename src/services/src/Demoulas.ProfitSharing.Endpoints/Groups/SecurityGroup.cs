using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class SecurityGroup : Group
{
    private const string Route = "security";
    private const string RouteName = "Security";
    public SecurityGroup()
    {
        //security is the route prefix for the top level group
        Configure(Route.ToLowerInvariant(), ep =>
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
