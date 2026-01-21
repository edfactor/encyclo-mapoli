namespace Demoulas.ProfitSharing.Common.Contracts.Request.Administration;

public sealed record CreateTaxCodeRequest
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public bool IsAvailableForDistribution { get; init; }

    public bool IsAvailableForForfeiture { get; init; }

    public bool IsProtected { get; init; }

    public static CreateTaxCodeRequest RequestExample()
    {
        return new CreateTaxCodeRequest
        {
            Id = "X",
            Name = "Example Tax Code",
            IsAvailableForDistribution = true,
            IsAvailableForForfeiture = false,
            IsProtected = false
        };
    }
}
