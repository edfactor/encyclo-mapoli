using Demoulas.ProfitSharing.Services.Serialization;
using Microsoft.AspNetCore.Builder;

namespace Demoulas.ProfitSharing.Services.Extensions;

public static class RoleContextApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSensitiveValueMasking(this IApplicationBuilder app)
        => app.UseMiddleware<RoleContextMiddleware>();
}
