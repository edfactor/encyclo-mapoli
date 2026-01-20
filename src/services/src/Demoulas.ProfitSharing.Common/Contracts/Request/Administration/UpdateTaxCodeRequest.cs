namespace Demoulas.ProfitSharing.Common.Contracts.Request.Administration;

public sealed record UpdateTaxCodeRequest
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public bool IsAvailableForDistribution { get; init; }

    public bool IsAvailableForForfeiture { get; init; }

    public bool IsProtected { get; init; }

    public static UpdateTaxCodeRequest RequestExample()
    {
        return new UpdateTaxCodeRequest
        {
            Id = "7",
            Name = "Normal distribution",
            IsAvailableForDistribution = true,
            IsAvailableForForfeiture = true,
            IsProtected = true
        };
    }
}
