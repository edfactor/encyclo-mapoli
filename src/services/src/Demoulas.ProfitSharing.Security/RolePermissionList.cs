using System.Collections.Immutable;
using Demoulas.Security;

namespace Demoulas.ProfitSharing.Security;
public static class RolePermissionList
{
    private static ImmutableList<RolePermissionModel>? _allRolePermissions = null;
    //XTODO: These entries are based on guesses, and need validation from business
    public static ImmutableList<RolePermissionModel> All
    {
        get
        {
            if (_allRolePermissions == null)
            {
                var rslt = new List<RolePermissionModel>();
                rslt.AddRange(AdminRoles);
                rslt.AddRange(FinanceManagerRoles);
                rslt.AddRange(DistributionClerkRoles);
                rslt.AddRange(HardshipAdministatorRoles);
                _allRolePermissions = rslt.ToImmutableList();
            }
            
            return _allRolePermissions;
        }
    }

    public static readonly List<RolePermissionModel> AdminRoles = [
        new RolePermissionModel {Role=Role.ADMINISTRATOR, Permission = Permission.YEARENDREPORTS}
    ];

    public static readonly List<RolePermissionModel> FinanceManagerRoles = [
        new RolePermissionModel {Role = Role.FINANCEMANAGER, Permission = Permission.YEARENDREPORTS}
    ];

    public static readonly List<RolePermissionModel> DistributionClerkRoles = [];
    public static readonly List<RolePermissionModel> HardshipAdministatorRoles = [];
}
