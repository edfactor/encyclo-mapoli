using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class SecurityGroup : Group
{
    private const string ROUTE = "security";
    private const string ROUTE_NAME = "Security";
    public SecurityGroup()
    {
        //security is the route prefix for the top level group
        Configure(ROUTE.ToLowerInvariant(), ep =>
        {
            ep.Description(x => x
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>()
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(500)
                .WithTags(ROUTE_NAME));
        });
    }
}
