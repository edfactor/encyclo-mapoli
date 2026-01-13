using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Military;

public sealed record GetMilitaryContributionRequest : SortedPaginationRequestDto, IBadgeNumberRequest
{
    public int BadgeNumber { get; init; }

    public static GetMilitaryContributionRequest RequestExample()
    {
        return new GetMilitaryContributionRequest
        {
            BadgeNumber = 1001,
            Skip = 0,
            Take = 50,
            SortBy = "Year",
            IsSortDescending = true
        };
    }
}
