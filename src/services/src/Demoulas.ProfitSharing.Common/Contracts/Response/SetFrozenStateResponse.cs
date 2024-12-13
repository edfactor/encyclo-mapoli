using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public sealed class SetFrozenStateResponse
{
    public int Id { get; set; }
    public short ProfitYear { get; set; }
}
