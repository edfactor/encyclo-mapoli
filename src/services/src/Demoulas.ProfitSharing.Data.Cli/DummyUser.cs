using System.Security.Claims;
using Demoulas.Common.Contracts.Interfaces;

namespace Demoulas.ProfitSharing.Data.Cli;
public sealed class DummyUser: IAppUser
{
    public bool HasRole(string roleName)
    {
        throw new NotImplementedException();
    }

    public bool HasPermission(string permissionName)
    {
        throw new NotImplementedException();
    }

    public List<string> GetUserAllRoles(List<string>? customRoles = null)
    {
        throw new NotImplementedException();
    }

    public List<string> GetUserAllPermissions(List<string>? customRoles = null)
    {
        throw new NotImplementedException();
    }

    public Claim? GetCustomClaim(string? claimType)
    {
        throw new NotImplementedException();
    }

    public int? GetStoreIDFromStoreClaim()
    {
        throw new NotImplementedException();
    }

    public string? UserName => "System-Cli";
    public string? Email { get; }
    public bool IsHQUser { get; }
    public int? StoreId { get; }
}
