using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Navigations;
public class UpdateNavigationRequestDto
{
    public int NavigationId { get; set; }
    public byte StatusId { get; set; }
}
