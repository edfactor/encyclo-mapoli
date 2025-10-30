using Demoulas.Common.Api.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class AdhocReportsGroup : GroupBase
{
    protected override string Route => "adhoc";
    protected override string RouteName => "Adhoc Reports";

    public AdhocReportsGroup()
    {
        Configure(Route.ToLowerInvariant(), ep =>
        {
            ep.Description(x => x
                .ProducesProblemFE<ProblemDetails>()
                .Produces<object>(StatusCodes.Status200OK)
                .Produces<object>(StatusCodes.Status400BadRequest)
                .Produces<object>(StatusCodes.Status401Unauthorized)
                .Produces<object>(StatusCodes.Status403Forbidden)
                .Produces<object>(StatusCodes.Status404NotFound)
                .Produces<object>(StatusCodes.Status500InternalServerError)
                .WithTags("Adhoc Reports"));
            ep.Policies(Policy.CanRunMasterInquiry);
        });
    }
}
