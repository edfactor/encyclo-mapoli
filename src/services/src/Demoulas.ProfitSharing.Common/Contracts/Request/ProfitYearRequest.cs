using System.ComponentModel;
using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record ProfitYearRequest : PaginationRequestDto
{
    [DefaultValue(2023)]
    public short ProfitYear { get; set; }


    public static ProfitYearRequest RequestExample()
    {
        return new ProfitYearRequest
        {
            ProfitYear = 2023,
            Skip = 0,
            Take = 25
        };
    }
}
