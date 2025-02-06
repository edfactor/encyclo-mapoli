using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record YearRequest
{
    [DefaultValue(2024)]
    public short ProfitYear { get; set; }
}
