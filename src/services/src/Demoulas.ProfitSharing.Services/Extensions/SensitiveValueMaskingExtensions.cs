using Demoulas.ProfitSharing.Services.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Demoulas.ProfitSharing.Services.Extensions;

public static class SensitiveValueMaskingExtensions
{
    public static IApplicationBuilder UseSensitiveValueMasking(this IApplicationBuilder app)
        => app.UseMiddleware<SensitiveValueMaskingMiddleware>();
}
