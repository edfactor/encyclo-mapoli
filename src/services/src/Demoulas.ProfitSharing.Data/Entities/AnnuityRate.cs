using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class AnnuityRate: ModifiedBase
{
    public int Id { get; set; }
    public short Year { get; set; }
    public byte Age { get; set; }
    public decimal SingleRate { get; set; }
    public decimal JointRate { get; set; }
}
