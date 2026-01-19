using Demoulas.Common.Api.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Builder;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class MilitaryGroup : GroupBase
{
    protected override string Route => "military";
    protected override string RouteName => "Military";

    public MilitaryGroup()
    {
        Configure(Route.ToLowerInvariant(), ep =>
        {
            ep.Description(x => x
                .ProducesProblemFE<ProblemDetails>()
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status401Unauthorized)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status500InternalServerError)
                .WithRequestTimeout(TimeSpan.FromMinutes(1))
                .WithTags(RouteName));
            // Military reports/records are read-only views for finance and auditors under year-end visibility.
            ep.Policies(Policy.CanViewYearEndReports);
        });
    }
}
