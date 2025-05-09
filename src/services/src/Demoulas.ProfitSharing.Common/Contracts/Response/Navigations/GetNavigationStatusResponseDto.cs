using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
public class GetNavigationStatusResponseDto
{
    public List<NavigationStatusDto>? NavigationStatusList { get; set; }
}
