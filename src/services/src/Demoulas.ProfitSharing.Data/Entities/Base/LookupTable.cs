using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Data.Entities.Base;
public abstract class LookupTable<TType>
{
    public required TType Id { get; set; }
    public required string Name { get; set; }
}
