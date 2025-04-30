using System.Collections.Immutable;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Contracts.Interfaces;

namespace Demoulas.ProfitSharing.Security;
public sealed class RolePermissionService : IRolePermissionService
{
    public ImmutableList<RolePermissionModel> GetAllRolePermissions()
    {
        return RolePermissionList.All;
    }
}
