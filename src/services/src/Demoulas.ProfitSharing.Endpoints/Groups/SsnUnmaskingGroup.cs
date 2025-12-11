using Demoulas.Common.Api.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

/// <summary>
/// FastEndpoints group for SSN unmasking endpoints.
/// Route prefix: /ssn-unmasking
/// 
/// These endpoints are strictly protected by the CanUnmaskSsn policy and can only be
/// accessed by users with the SSN-Unmasking role. This ensures that sensitive SSN data
/// is only revealed to authorized compliance and verification staff.
/// </summary>
public sealed class SsnUnmaskingGroup : GroupBase
{
    protected override string RouteName => "SsnUnmasking";
    protected override string Route => "ssn-unmasking";

    public SsnUnmaskingGroup()
    {
        Configure(Route.ToLowerInvariant(), ep =>
        {
            ep.Description(x => x
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>()
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(StatusCodes.Status401Unauthorized)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<Microsoft.AspNetCore.Mvc.ProblemDetails>(StatusCodes.Status500InternalServerError)
                .WithRequestTimeout(TimeSpan.FromSeconds(30))
                .WithTags(RouteName));
            ep.Policies(Policy.CanUnmaskSsn);
        });
    }
}
