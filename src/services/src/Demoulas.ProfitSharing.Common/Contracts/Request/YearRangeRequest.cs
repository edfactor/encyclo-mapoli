using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record YearRangeRequest
{
    [DefaultValue(2024)]
    public short BeginProfitYear { get; set; }
    public short EndProfitYear { get; set; }

    public static YearRangeRequest RequestExample() => new()
    {
        BeginProfitYear = 2023,
        EndProfitYear = 2024
    };
}
