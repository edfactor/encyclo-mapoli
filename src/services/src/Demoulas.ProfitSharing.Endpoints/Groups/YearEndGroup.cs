using Demoulas.Common.Api.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

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
                .ProducesProblemFE<ProblemDetails>()
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status401Unauthorized)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status500InternalServerError)
                // 2-minute timeout for year-end reports (target: 95% complete in < 90 seconds)
                // CRITICAL: Requires AddYearEndPerformanceIndexes migration deployed FIRST
                // Without indexes: queries will timeout (5-19 minutes observed in UAT)
                // With indexes: < 2 minutes expected for 10,000+ employee datasets
                .WithRequestTimeout(TimeSpan.FromMinutes(2))
                .WithTags(RouteName));
            ep.Policies(Policy.CanViewYearEndReports);
        });
    }
}
