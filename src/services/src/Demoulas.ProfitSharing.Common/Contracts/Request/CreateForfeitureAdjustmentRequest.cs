using System;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request to create a new forfeiture adjustment
/// </summary>
public record CreateForfeitureAdjustmentRequest
{
    public required int ClientNumber { get; init; }
    public required int BadgeNumber { get; init; }
    public required decimal StartingBalance { get; init; }
    public required decimal ForfeitureAmount { get; init; }
    public required decimal NetBalance { get; init; }
    public required decimal NetVested { get; init; }
    public int ProfitYear { get; init; }

    public static CreateForfeitureAdjustmentRequest RequestExample()
    {
        return new CreateForfeitureAdjustmentRequest
        {
            ClientNumber = 1,
            BadgeNumber = 1234567890,
            StartingBalance = 5000,
            ForfeitureAmount = 1000,
            NetBalance = 4000,
            NetVested = 3000,
            ProfitYear = 2024
        };
    }
} 