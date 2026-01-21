namespace Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;

public sealed record TaxCodeLookupRequest
{
    public bool? AvailableForDistribution { get; init; }

    public bool? AvailableForForfeiture { get; init; }

    public static TaxCodeLookupRequest RequestExample()
    {
        return new TaxCodeLookupRequest
        {
            AvailableForDistribution = true,
            AvailableForForfeiture = true
        };
    }
}
