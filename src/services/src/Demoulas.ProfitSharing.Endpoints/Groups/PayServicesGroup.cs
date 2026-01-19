using Demoulas.Common.Api.Groups;
using Demoulas.ProfitSharing.Security;  // Add this using
using Microsoft.AspNetCore.Builder;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class PayServicesGroup : GroupBase
{
    protected override string RouteName => "PayServices";
    protected override string Route => "payservices";

    public PayServicesGroup()
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

            // PS-868: Group-level policy authorization for pay services viewing
            ep.Policies(Policy.CanViewYearEndReports);
        });
    }
}
