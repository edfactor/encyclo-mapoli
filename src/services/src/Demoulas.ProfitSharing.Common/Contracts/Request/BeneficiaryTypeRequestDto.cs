using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public sealed record BeneficiaryTypeRequestDto
{
    public required byte Id { get; set; }
    public required string Name { get; set; }
}
