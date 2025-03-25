
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public sealed record RehireForfeituresRequest : ProfitYearRequest
{
    public required DateOnly BeginningDate { get; set; }
    public required DateOnly EndingDate { get; set; }



    public static new RehireForfeituresRequest RequestExample()
    {
        return new RehireForfeituresRequest
        {
            BeginningDate = new DateOnly(2024, 03, 15),
            EndingDate = new DateOnly(2024, 09, 15),
            Skip = 1,
            Take = 10,
            SortBy = "BadgeNumber",
            IsSortDescending = false,
            ProfitYear = 2024
        };
    }
}
