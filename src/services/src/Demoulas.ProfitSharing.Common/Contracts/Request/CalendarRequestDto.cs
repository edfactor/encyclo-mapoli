using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record CalendarRequestDto
{
    [DefaultValue(2023)]
    public short ProfitYear { get; set; }
}
