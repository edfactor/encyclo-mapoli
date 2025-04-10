using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
public class NavigationStatusTracking
{
    public int Id { get; set; }
    public int NavigationId { get; set; }
    public int StatusId { get; set; }
    public required string Username { get; set; }
    public required DateTime LastModifiedDate { get; set; }

}
