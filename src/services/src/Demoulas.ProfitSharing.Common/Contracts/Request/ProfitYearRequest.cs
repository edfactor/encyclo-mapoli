using System.ComponentModel;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record ProfitYearRequest : SortedPaginationRequestDto, IProfitYearRequest
{
    [DefaultValue(2024)]
    public short ProfitYear { get; set; }


    public static ProfitYearRequest RequestExample()
    {
        return new ProfitYearRequest
        {
            ProfitYear = 2024,
            Skip = 0,
            Take = 25
        };
    }
}
