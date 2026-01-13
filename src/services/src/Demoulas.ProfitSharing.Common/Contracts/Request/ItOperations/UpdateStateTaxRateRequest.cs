namespace Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;

public sealed record UpdateStateTaxRateRequest
{
    public required string Abbreviation { get; init; }

    public decimal Rate { get; init; }

    public static UpdateStateTaxRateRequest RequestExample()
    {
        return new UpdateStateTaxRateRequest
        {
            Abbreviation = "MA",
            Rate = 0.05m
        };
    }
}
