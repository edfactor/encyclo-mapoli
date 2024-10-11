using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Demoulas.ProfitSharing.Security;
using System.Net;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class YearEndGroup : Group
{
    private const string Route = "yearend";
    private const string RouteName = "Year End";
    public YearEndGroup(Action<RouteHandlerBuilder> builder)
    {
        Configure(Route.ToLowerInvariant(), ep =>
        {
            ep.Description(x =>
            {
                // Apply chained methods to x
                x.Produces((int)HttpStatusCode.Unauthorized)
                    .Produces((int)HttpStatusCode.Forbidden)
                    .Produces((int)HttpStatusCode.NotFound)
                    .Produces((int)HttpStatusCode.TooManyRequests)
                    .Produces((int)HttpStatusCode.MethodNotAllowed)
                    .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>()
                    .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>((int)HttpStatusCode.InternalServerError)
                    .WithRequestTimeout(TimeSpan.FromMinutes(1))
                    .WithTags(RouteName);

                // Now invoke the builder, passing x (the RouteHandlerBuilder instance)
                builder(x);
            });

            ep.Policies(Policy.CanViewYearEndReports);
        });
    }
}
