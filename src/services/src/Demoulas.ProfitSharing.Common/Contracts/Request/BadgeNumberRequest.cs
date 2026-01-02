namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record BadgeNumberRequest : FrozenProfitYearRequest
{
    public int? BadgeNumber { get; set; }
    // All other filtering properties removed for simplification

    public static new BadgeNumberRequest RequestExample() => new()
    {
        ProfitYear = 2024,
        BadgeNumber = 123456,
        UseFrozenData = false
    };
}
