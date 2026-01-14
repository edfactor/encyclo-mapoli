using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Military;

[YearEndArchiveProperty]
public sealed record MilitaryContributionResponse : YearRequest, IIsExecutive
{
    public int BadgeNumber { get; init; }
    public decimal Amount { get; init; }
    public byte? CommentTypeId { get; init; }
    public bool IsSupplementalContribution { get; init; }
    public DateOnly ContributionDate { get; set; }
    public bool IsExecutive { get; set; }

    public static MilitaryContributionResponse ResponseExample()
    {
        return new MilitaryContributionResponse
        {
            ProfitYear = 2024,
            BadgeNumber = 12345,
            Amount = 5000.00m,
            CommentTypeId = 1,
            IsSupplementalContribution = false,
            ContributionDate = DateOnly.FromDateTime(DateTime.Today),
            IsExecutive = false
        };
    }
}
