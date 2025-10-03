using System.Security.Claims;
using System.Text.RegularExpressions;
using Demoulas.Common.Contracts.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Security;

public sealed class ImpersonationAndEnvironmentAwareClaimsTransformation : IClaimsTransformation
{
    private readonly OktaConfiguration _oktaConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private const string oktaRoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

    public ImpersonationAndEnvironmentAwareClaimsTransformation(OktaConfiguration oktaConfiguration, IHttpContextAccessor httpContextAccessor)
    {
        _oktaConfiguration = oktaConfiguration;
        _httpContextAccessor = httpContextAccessor;
    }
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var roles = new List<string>();

        foreach (var val in principal.Claims.Where(x => x.Type == "groups"))
        {
            var roleName = EnvironmentSensitiveRoleName(val.Value);
            if (!string.IsNullOrEmpty(roleName))
            {
                roles.Add(roleName);
            }
        }

        if (string.Compare(GetEnvironment(), "Prod", StringComparison.OrdinalIgnoreCase) != 0 && roles.Contains(Role.IMPERSONATION))
        {
            roles = roles.Union(GetImpersonationRoles()).ToList();
        }

        principal.AddIdentity(new(roles.Select(p => new Claim(oktaRoleClaimType, p))));


        return Task.FromResult(principal);
    }

    private string EnvironmentSensitiveRoleName(string roleName)
    {
        var alphaDashRegex = new Regex("^[a-z0-9-]+", RegexOptions.IgnoreCase);
        if (alphaDashRegex.IsMatch(roleName))
        {
            var prefixEnvRegex = new Regex($"^{_oktaConfiguration.RolePrefix}-{GetEnvironment()}-([a-z0-9-]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var prefixEnvRegexMatch = prefixEnvRegex.Match(roleName);
            if (prefixEnvRegexMatch.Success)
            {
                return prefixEnvRegexMatch.Groups[1].Value;
            }
        }

        return string.Empty;
    }

    private string GetEnvironment()
    {
        if (!string.IsNullOrWhiteSpace(_oktaConfiguration.EnvironmentName))
        {
            return _oktaConfiguration.EnvironmentName;
        }

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        // Development and QA are using QA.
        // UAT and Production are using Prod.
        switch (environment.ToUpper())
        {
            case "TESTING":
                return "Testing";
            case "DEVELOPMENT":
            case "QA":
                return "QA";
            case "UAT":
            case "PRODUCTION":
                return "Prod";
            default:
                return "";
        }
    }

    private List<string> GetImpersonationRoles()
    {
        var roles = new List<string>();

        string? headerValue = _httpContextAccessor.HttpContext?.Request.Headers[Role.IMPERSONATION];
        if (!string.IsNullOrEmpty(headerValue))
        {
            return headerValue.Split("|", StringSplitOptions.TrimEntries)
              .Select(r => r.Replace(_oktaConfiguration.RolePrefix, ""))
              .ToList();
        }

        return roles;
    }
}
