using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Security;

namespace Demoulas.ProfitSharing.Security;
public sealed class RolePermissionService : IRolePermissionService
{
    public ImmutableList<RolePermissionModel> GetAllRolePermissions()
    {
        return RolePermissionList.All;
    }
}
