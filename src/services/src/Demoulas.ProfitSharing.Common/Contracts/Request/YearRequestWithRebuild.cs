
using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record YearRequestWithRebuild
{
    [DefaultValue(2024)]
    public short ProfitYear { get; set; }

    [DefaultValue(false)]
    public bool Rebuild { get; set; }
}
