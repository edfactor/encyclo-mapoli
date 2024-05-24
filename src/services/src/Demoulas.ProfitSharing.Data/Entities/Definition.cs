using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class Definition
{
    public required string Key { get; init; }
    public required string Description { get; init; }
}
