using Demoulas.Common.Api.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

/// <summary>
/// Group for validation endpoints (checksum validation, data integrity checks).
/// Restricted to IT DevOps and Administrator roles for sensitive operations.
/// </summary>
public sealed class ValidationGroup : GroupBase
{
    protected override string Route => "validation";
    protected override string RouteName => "Validation";

    public ValidationGroup()
    {
        Configure(Route.ToLowerInvariant(), ep =>
        {
            ep.Description(x => x
                .ProducesProblemFE<ProblemDetails>()
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status401Unauthorized)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status500InternalServerError)
                .WithRequestTimeout(TimeSpan.FromMinutes(2))
                .WithTags(RouteName));

            // Validation endpoints restricted to IT DevOps and Administrators
            ep.Roles(Role.ITDEVOPS, Role.ADMINISTRATOR);
        });
    }
}
