using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Data.Entities;
public class ProfitCode
{
    public required short Code { get; init; }
    public required string Definition { get; init; }
    public required string Frequency { get; init; }
}
