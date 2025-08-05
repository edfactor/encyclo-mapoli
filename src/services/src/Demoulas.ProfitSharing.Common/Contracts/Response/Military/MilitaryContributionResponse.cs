using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Military;

public sealed record MilitaryContributionResponse : YearRequest
{
    [YearEndArchiveProperty]
    public int BadgeNumber { get; init; }
    [YearEndArchiveProperty]
    public decimal Amount { get; init; }
    [YearEndArchiveProperty]
    public byte? CommentTypeId { get; init; }
    [YearEndArchiveProperty]
    public bool IsSupplementalContribution { get; init; }
    [YearEndArchiveProperty]
    public DateOnly ContributionDate { get; init; }
}
