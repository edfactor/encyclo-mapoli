namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request to update forfeiture adjustments for a given badge number and profit year.
/// </summary>
public record ForfeitureAdjustmentUpdateRequest
{
    public required int ClientNumber { get; init; }
    public required int BadgeNumber { get; init; }
    public required decimal ForfeitureAmount { get; init; }
    public string? Reason { get; init; }
    public int ProfitYear { get; init; }

    public static ForfeitureAdjustmentUpdateRequest RequestExample()
    {
        return new ForfeitureAdjustmentUpdateRequest
        {
            ClientNumber = 1,
            BadgeNumber = 1234567890,
            ForfeitureAmount = 1000,
            Reason = "Example reason",
            ProfitYear = 2024
        };
    }
}
