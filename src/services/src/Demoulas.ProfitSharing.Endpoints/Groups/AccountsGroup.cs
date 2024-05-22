using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class AccountsGroup : Group
{
    private const string ROUTE = "accounts";
    private const string ROUTE_NAME = "Accounts";
    public AccountsGroup()
    {
        Configure(ROUTE.ToLowerInvariant(), ep => //admin is the route prefix for the top level group
        {
            ep.Description(x => x
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>()
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(500)
                .WithTags(ROUTE_NAME));
        });
    }
}
