namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record SuggestedForfeitureAdjustmentResponse
{
    public int DemographicId { get; set; }
    public int BadgeNumber { get; set; }
    public decimal SuggestedForfeitAmount { get; set; }

    public static SuggestedForfeitureAdjustmentResponse ResponseExample()
    {
        return new SuggestedForfeitureAdjustmentResponse()
        {
            DemographicId = 44,
            BadgeNumber = 7,
            SuggestedForfeitAmount = 2000.21m
        };
    }
}
