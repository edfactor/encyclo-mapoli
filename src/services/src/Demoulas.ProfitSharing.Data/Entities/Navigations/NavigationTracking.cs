using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Data.Entities.Navigations;
public class NavigationTracking
{
    public int Id { get; set; }
    public int NavigationId { get; set; }
    public byte? StatusId { get; set; }
    public string? Username { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    public Navigation? Navigation { get; set; }
    public NavigationStatus? NavigationStatus { get; set; }

}
