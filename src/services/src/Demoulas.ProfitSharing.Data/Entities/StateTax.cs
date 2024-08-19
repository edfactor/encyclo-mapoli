using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Data.Entities;

public class StateTax
{
    public required string Abbreviation { get; set; }
    public required decimal Rate { get; set; }       
    public required string UserModified { get; set; }
    public required DateOnly DateModified { get; set; }

}
