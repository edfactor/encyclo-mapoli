using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record YearRangeRequest
{
    [DefaultValue(2024)]
    public short BeginProfitYear { get; set; }
    public short EndProfitYear { get; set; }
}
