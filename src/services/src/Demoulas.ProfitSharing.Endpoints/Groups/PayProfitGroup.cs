using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class PayProfitGroup : Group
{
    private const string Route = "payprofit";
    private const string RouteName = "PayProfit";
    public PayProfitGroup()
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
