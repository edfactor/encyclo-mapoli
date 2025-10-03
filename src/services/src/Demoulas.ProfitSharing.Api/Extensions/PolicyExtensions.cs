using Demoulas.ProfitSharing.Api.Security;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Api.Extensions;

internal static class PolicyExtensions
{
    internal static WebApplicationBuilder ConfigureSecurityPolicies(this WebApplicationBuilder builder)
    {

        _ = builder.Services.AddAuthorization(options =>
        {
            foreach (var kvp in PolicyRoleMap.Map)
            {
                var policyName = kvp.Key;
                var roles = kvp.Value;
                options.AddPolicy(policyName, x => x.RequireRole(roles));
            }
        });

        return builder;
    }
}
