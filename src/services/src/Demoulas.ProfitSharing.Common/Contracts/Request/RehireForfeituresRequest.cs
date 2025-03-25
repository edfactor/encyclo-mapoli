
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
            BeginningDate = DateTime.Today.AddYears(-2).ToDateOnly(),
            EndingDate = DateTime.Today.AddYears(-1).ToDateOnly(),
            Skip = 1,
            Take = 10,
            SortBy = "BadgeNumber",
            IsSortDescending = false,
            ProfitYear = 2024
        };
    }
}
