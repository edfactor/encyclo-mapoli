using Demoulas.ProfitSharing.Api.Middleware;

namespace Demoulas.ProfitSharing.Api.Extensions;

public static class VersionMiddlewareExtensions
{
    public static IApplicationBuilder UseVersionHeader(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<VersionMiddleware>();
    }
}
