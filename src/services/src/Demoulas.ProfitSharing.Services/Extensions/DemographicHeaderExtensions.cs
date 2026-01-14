using Demoulas.ProfitSharing.Services.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Demoulas.ProfitSharing.Services.Extensions;

public static class DemographicHeaderExtensions
{
    public static IApplicationBuilder UseDemographicHeaders(
        this IApplicationBuilder app)
        => app.UseMiddleware<DemographicHeaderMiddleware>();
}
