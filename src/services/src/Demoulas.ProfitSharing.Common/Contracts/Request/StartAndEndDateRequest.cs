

using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record StartAndEndDateRequest : SortedPaginationRequestDto
{
    public DateOnly BeginningDate { get; set; }
    public DateOnly EndingDate { get; set; }

    public static StartAndEndDateRequest RequestExample()
    {
        return new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2019, 01, 01),
            EndingDate = new DateOnly(2024, 12, 31),
            Skip = 1,
            Take = 10,
            SortBy = "BadgeNumber",
            IsSortDescending = false
        };
    }
}
