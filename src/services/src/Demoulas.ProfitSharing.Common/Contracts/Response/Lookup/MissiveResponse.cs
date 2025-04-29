using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
public sealed record MissiveResponse
{
    public int Id { get; set; }
    public required string Message { get; set; }
}
