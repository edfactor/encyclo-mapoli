namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request to update forfeiture adjustments for a given badge number and profit year.
/// </summary>
public record ForfeitureAdjustmentUpdateRequest
{
    public required int BadgeNumber { get; init; }
    public required decimal ForfeitureAmount { get; init; }
    public string? Reason { get; init; }
    public int ProfitYear { get; init; }
    public int? OffsettingProfitDetailId { get; set; } // This states which profit detail record the user is trying to offset against.

    public static ForfeitureAdjustmentUpdateRequest RequestExample()
    {
        return new ForfeitureAdjustmentUpdateRequest
        {
            BadgeNumber = 1234567890,
            ForfeitureAmount = 1000,
            Reason = "Example reason",
            ProfitYear = 2024
        };
    }
}
