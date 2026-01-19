using Demoulas.Common.Api.Groups;
using Microsoft.AspNetCore.Builder;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class ProfitDetailsGroup : GroupBase
{
    protected override string RouteName => "ProfitDetails";
    protected override string Route => "profitdetails";

    public ProfitDetailsGroup()
    {
        Configure(Route.ToLowerInvariant(), ep =>
        {
            ep.Description(x => x
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>()
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(StatusCodes.Status401Unauthorized)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(StatusCodes.Status500InternalServerError)
                .WithRequestTimeout(TimeSpan.FromMinutes(1))
                .WithTags(RouteName));
        });
    }
}
