namespace Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;

public sealed record StateTaxLookupRequest
{
    public required string State { get; init; }
}
