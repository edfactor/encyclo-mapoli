using System.Security.Claims;
using Demoulas.Common.Contracts.Interfaces;

namespace Demoulas.ProfitSharing.OracleHcm.Contracts;

public sealed class DummyUser : IAppUser
{
    public bool HasRole(string roleName)
    {
        return false;
    }

    public bool HasPermission(string permissionName)
    {
        var allPermissions = GetUserAllPermissions();
        return allPermissions.Contains(permissionName);
    }

    public List<string> GetUserAllRoles(List<string>? customRoles = null)
    {
        var roles = new List<string>();

        if (customRoles != null)
        {
            roles.AddRange(customRoles);
        }

        return roles;
    }

    public List<string> GetUserAllPermissions(List<string>? customRoles = null)
    {
        var permissions = new List<string>();
        var roles = GetUserAllRoles(customRoles);

        foreach (var role in roles)
        {
            // Assuming each role maps to a set of permissions
            // Add logic here to fetch permissions for the role
            // For now, we'll simulate with dummy data
            permissions.Add($"{role}_Permission1");
            permissions.Add($"{role}_Permission2");
        }

        return permissions.Distinct().ToList();
    }

    public Claim? GetCustomClaim(string? claimType)
    {
        if (string.IsNullOrEmpty(claimType))
        {
            return null;
        }

        var claimsIdentity = ClaimsPrincipal.Current?.Identity as ClaimsIdentity;
        return claimsIdentity?.FindFirst(claimType);
    }

    public int? GetStoreIDFromStoreClaim()
    {
        var storeClaim = GetCustomClaim("store_id");
        if (storeClaim != null && int.TryParse(storeClaim.Value, out var storeId))
        {
            return storeId;
        }

        return null;
    }

    public string? UserName => "System";
    public string? Email { get; }
    public bool IsHQUser { get; }
    public int? StoreId { get; }
}
