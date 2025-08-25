using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Military;

[YearEndArchiveProperty]
public sealed record MilitaryContributionResponse : YearRequest
{
    public int BadgeNumber { get; init; }
    public decimal Amount { get; init; }
    public byte? CommentTypeId { get; init; }
    public bool IsSupplementalContribution { get; init; }
    public DateOnly ContributionDate { get; init; }
    public bool IsExecutive { get; init; }
}
