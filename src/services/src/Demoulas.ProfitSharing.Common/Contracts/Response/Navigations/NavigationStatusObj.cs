using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
public record NavigationStatusObj
{
    public int Id { get; set; }
    public string? Name { get; set; }
}
