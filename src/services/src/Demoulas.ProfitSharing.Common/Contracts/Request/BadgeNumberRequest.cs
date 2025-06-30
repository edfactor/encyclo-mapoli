namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record BadgeNumberRequest : FrozenProfitYearRequest
{
    public int? BadgeNumber { get; set; }
    // All other filtering properties removed for simplification
}
