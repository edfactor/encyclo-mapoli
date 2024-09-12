using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class YearEndGroup : Group
{
    private const string Route = "yearend";
    private const string RouteName = "Year End";
    public YearEndGroup()
    {
        Configure(Route.ToLowerInvariant(), ep => //admin is the route prefix for the top level group
        {
            ep.Description(x => x
                .Produces(401)
                .Produces(403)
                .Produces(429)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>()
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(500)
                .WithRequestTimeout(TimeSpan.FromMinutes(1))
                .WithTags(RouteName));
            ep.Policies(Policy.CanViewYearEndReports);
            ep.Throttle(30, 60); });
    }
}
