using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
public static class NavigationStatusConstant
{
    public const byte NotStarted = 0;
    public const byte InProgress = 1;
    public const byte Blocked = 2;
    public const byte Successful = 3;
}
