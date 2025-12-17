
using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record YearRequestWithRebuild : YearRequest
{
    [DefaultValue(false)]
    public bool Rebuild { get; set; }
}
