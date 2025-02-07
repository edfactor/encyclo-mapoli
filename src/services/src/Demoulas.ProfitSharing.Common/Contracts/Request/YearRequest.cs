using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record YearRequest
{
    [DefaultValue(2024)]
    public short ProfitYear { get; set; }
}
