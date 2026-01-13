using Demoulas.Common.Api.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

/// <summary>
/// Groups endpoints related to demographic operations
/// </summary>
public sealed class AdjustmentsGroup : GroupBase
{
    public AdjustmentsGroup() : base()
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
            ep.Policies(Policy.CanRunMasterInquiry);
        });
    }

    protected override string Route => "adjustments";
    protected override string RouteName => "Adjustments";
}
