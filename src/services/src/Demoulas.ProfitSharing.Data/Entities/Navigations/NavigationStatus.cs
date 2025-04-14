using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Data.Entities.Navigations;
public class NavigationStatus
{
    [Key]
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }

    public List<Navigation>? Navigations { get; set; }
    public List<NavigationTracking>? NavigationTrackings { get; set; }

}
