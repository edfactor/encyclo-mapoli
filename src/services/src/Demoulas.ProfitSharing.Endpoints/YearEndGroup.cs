using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Demoulas.ProfitSharing.Endpoints;
public sealed class YearEndGroup : Group
{
    private const string Route = "yearend";
    private const string RouteName = "YearEnd";

    public YearEndGroup()
    {
        Configure(Route.ToLowerInvariant(), ep => 
        {
            ep.Description(x => x
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>()
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(500)
                .WithTags(RouteName));
        });
    }
}
