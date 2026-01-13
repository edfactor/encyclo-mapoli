using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record YearRequest : IProfitYearRequest
{
    [DefaultValue(2024)]
    public short ProfitYear { get; set; }

    public static YearRequest RequestExample() => new() { ProfitYear = 2024 };
}
