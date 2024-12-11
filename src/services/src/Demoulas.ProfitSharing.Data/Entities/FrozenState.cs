using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class FrozenState
{
    public int Id { get; set; }
    public short ProfitYear { get; set; }
    public bool IsActive { get; set; }
    public DateTime AsOfDateTime { get; set; }
}
