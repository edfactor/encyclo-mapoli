using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Data.Entities.Navigations;
public class NavigationRole
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Navigation>? Navigations { get; set; }
}
