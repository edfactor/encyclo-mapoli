using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Data.Entities;
public class TaxCode
{
    public char Code { get; set; }
    public required string Description { get; set; }
}
