using Demoulas.Common.Api.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

/// <summary>
/// FastEndpoints group for check run operations.
/// Routes: /check-run/*
/// Authorization: Policy.CanProcessChecks (Finance Manager, Administrator, Distributions Clerk, IT DevOps, Hardship Administrator)
/// </summary>
public sealed class CheckRunGroup : GroupBase
{
    protected override string Route => "check-run";
    protected override string RouteName => "Check Run";

    public CheckRunGroup()
    {
        Configure(Route.ToLowerInvariant(), ep =>
        {
            ep.Description(x => x
                .ProducesProblemFE<ProblemDetails>()
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status401Unauthorized)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status500InternalServerError)
                .WithTags(RouteName));
            ep.Policies(Policy.CanProcessChecks);
        });
    }
}
