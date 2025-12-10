namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record FrozenProfitYearRequest : ProfitYearRequest
{
    public bool UseFrozenData { get; set; }
}
