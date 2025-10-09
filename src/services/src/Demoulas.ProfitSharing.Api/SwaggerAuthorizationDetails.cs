using Demoulas.ProfitSharing.Api.Security;
using Microsoft.AspNetCore.Authorization;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Demoulas.ProfitSharing.Api;

/// <summary>
/// Adds authorization details (required policies and allowed roles) to each Swagger operation.
/// Information is derived from endpoint metadata and registered AuthorizationOptions.
/// </summary>
public sealed class SwaggerAuthorizationDetails : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        // NSwag doesn't expose EndpointMetadata directly across all pipelines. Instead, infer from attributes on the controller/method.
        var method = context.MethodInfo;
        var declaringType = method.DeclaringType;
        var authorizeData = new List<IAuthorizeData>();

        // 1) Preferred: pull from ASP.NET Core EndpointMetadata (covers Minimal APIs and FastEndpoints group-level auth)
        if (context is AspNetCoreOperationProcessorContext aspCtx)
        {
            var endpointMetadata = aspCtx.ApiDescription?.ActionDescriptor?.EndpointMetadata;
            if (endpointMetadata is not null)
            {
                // If AllowAnonymous is present, treat as no auth
                if (endpointMetadata.OfType<IAllowAnonymous>().Any() || endpointMetadata.OfType<AllowAnonymousAttribute>().Any())
                {
                    return true;
                }

                authorizeData.AddRange(endpointMetadata.OfType<IAuthorizeData>());
            }
        }

        // 2) Fallback: legacy MVC reflection on controller/method
        if (authorizeData.Count == 0)
        {
            if (declaringType is not null)
            {
                authorizeData.AddRange(declaringType.GetCustomAttributes(inherit: true).OfType<IAuthorizeData>());
            }
            authorizeData.AddRange(method.GetCustomAttributes(inherit: true).OfType<IAuthorizeData>());
        }

        if (authorizeData.Count == 0)
        {
            // No authorization metadata (either anonymous or auth-only without explicit policy/roles metadata surfaced here).
            return true;
        }

        // Collect distinct policy names
        var policies = authorizeData
            .Select(d => d.Policy)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct()
            .ToList();

        // Build map of policy -> allowed roles from centralized map
        var rolesByPolicy = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        if (policies.Count > 0)
        {
            foreach (var policyName in policies.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                rolesByPolicy[policyName!] = PolicyRoleMap.GetRoles(policyName!);
            }
        }


        var op = context.OperationDescription.Operation;

        // Add vendor extension for machine-readability
        op.ExtensionData ??= new Dictionary<string, object?>();
        op.ExtensionData["x-authorization"] = new
        {
            authenticationRequired = true,
            policies = rolesByPolicy
        };

        // Append a short, human-readable note to the description
        var lines = new List<string>();
        if (policies.Count == 0)
        {
            if (rolesByPolicy.TryGetValue("[none]", out var roles) && roles.Length > 0)
            {
                lines.Add($"Authorization: Authentication required; allowed roles: {string.Join(", ", roles)}");
            }
            else
            {
                lines.Add("Authorization: Authentication required; no specific policy.");
            }
        }
        else
        {
            foreach (var kvp in rolesByPolicy.Where(p => p.Value.Any()))
            {
                var roles = kvp.Value;
                var rolesText = roles.Length > 0 ? string.Join(", ", roles) : "none";
                lines.Add($"Authorization: Policy '{kvp.Key}' (roles: {rolesText})");
            }
        }

        var append = "\n\n" + string.Join("\n", lines);
        if (string.IsNullOrWhiteSpace(op.Description))
        {
            op.Description = append.Trim();
        }
        else if (!op.Description.Contains("Authorization:"))
        {
            op.Description += append;
        }

        return true;
    }
}
