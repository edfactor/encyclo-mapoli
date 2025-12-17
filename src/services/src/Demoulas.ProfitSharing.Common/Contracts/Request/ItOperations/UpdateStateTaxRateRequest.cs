namespace Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;

public sealed record UpdateStateTaxRateRequest
{
    public required string Abbreviation { get; init; }

    public decimal Rate { get; init; }
}
