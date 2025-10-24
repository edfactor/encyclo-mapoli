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
            // Extract roles efficiently - pre-allocate collection size hint
            var roleClaims = context.User.Claims
                .Where(c => string.Equals(c.Type, System.Security.Claims.ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(c.Type, "roles", StringComparison.OrdinalIgnoreCase));

            // Use HashSet for efficient distinct operation, then convert to array
            var rolesSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var claim in roleClaims)
            {
                rolesSet.Add(claim.Value);
            }

            string[] roles = [.. rolesSet];

            // Check for specific roles using Span-based comparison
            bool isItDevOps = rolesSet.Contains(Role.ITDEVOPS);
            bool isExecAdmin = rolesSet.Contains(Role.EXECUTIVEADMIN);

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
