using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Services.Serialization;

/// <summary>
/// Middleware that populates <see cref="MaskingAmbientRoleContext.Current"/> from the authenticated user.
/// Placed early in the pipeline so serialization later can observe it.
/// </summary>
public sealed class RoleContextMiddleware
{
    private readonly RequestDelegate _next;
    public RoleContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            string[] roles = context.User.Claims
                .Where(c => string.Equals(c.Type, System.Security.Claims.ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(c.Type, "roles", StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            bool isItDevOps = roles.Any(r => string.Equals(r, Role.ITDEVOPS, StringComparison.OrdinalIgnoreCase));
            bool isExecAdmin = roles.Any(r => string.Equals(r, Role.EXECUTIVEADMIN, StringComparison.OrdinalIgnoreCase));
            MaskingAmbientRoleContext.Current = new RoleContextSnapshot(roles, isItDevOps, isExecAdmin);
        }
        try
        {
            await _next(context);
        }
        finally
        {
            MaskingAmbientRoleContext.Clear();
        }
    }
}