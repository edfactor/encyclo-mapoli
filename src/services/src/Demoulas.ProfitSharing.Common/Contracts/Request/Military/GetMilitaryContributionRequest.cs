using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.Military;

public sealed record GetMilitaryContributionRequest : SortedPaginationRequestDto
{
    public int BadgeNumber { get; init; }
}
