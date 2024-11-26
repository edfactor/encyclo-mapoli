using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Demoulas.ProfitSharing.Security;
using System.Net;
using Demoulas.Common.Api.Groups;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class YearEndGroup : GroupBase
{
    protected override string Route => "yearend";
    protected override string RouteName => "Year End";

    public YearEndGroup()
    {
        Configure(Route.ToLowerInvariant(), ep => //admin is the route prefix for the top level group
        {
            ep.Description(x => x
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>()
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(StatusCodes.Status401Unauthorized)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(StatusCodes.Status500InternalServerError)
                .WithRequestTimeout(TimeSpan.FromMinutes(1))
                .WithTags(RouteName));
            ep.Policies(Policy.CanViewYearEndReports);
        });
    }
}
