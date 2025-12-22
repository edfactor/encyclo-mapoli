using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Military;

public sealed record GetMilitaryContributionRequest : SortedPaginationRequestDto
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
