namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record FrozenProfitYearRequest : ProfitYearRequest
{
    public bool UseFrozenData { get; set; }

    public static new FrozenProfitYearRequest RequestExample()
    {
        return new FrozenProfitYearRequest
        {
            ProfitYear = 2024,
            UseFrozenData = true
        };
    }
}
