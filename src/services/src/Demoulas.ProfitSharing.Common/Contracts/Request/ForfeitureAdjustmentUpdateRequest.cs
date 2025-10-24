using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request to update forfeiture adjustments for a given badge number and profit year.
/// Use a positive forfeiture amount to forfeit money back to the plan, use an negative amount to unforforeit money back to the employee.
/// </summary>
public record ForfeitureAdjustmentUpdateRequest : IProfitYearRequest
{
    public required int BadgeNumber { get; init; }
    public required decimal ForfeitureAmount { get; init; }
    public bool ClassAction { get; init; }
    public short ProfitYear { get; set; }
    public int? OffsettingProfitDetailId { get; set; } // This states which profit detail record the user is trying to offset against.

    public static ForfeitureAdjustmentUpdateRequest RequestExample()
    {
        return new ForfeitureAdjustmentUpdateRequest
        {
            BadgeNumber = 1234567890,
            ForfeitureAmount = 1000,
            ClassAction = false,
            ProfitYear = 2024
        };
    }
}
