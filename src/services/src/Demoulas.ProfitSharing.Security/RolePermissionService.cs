using System.Collections.Immutable;
using Demoulas.Security;

namespace Demoulas.ProfitSharing.Security;
public sealed class RolePermissionService : IRolePermissionService
{
    public ImmutableList<RolePermissionModel> GetAllRolePermissions()
    {
        return RolePermissionList.All;
    }
}
