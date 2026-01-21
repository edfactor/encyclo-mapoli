namespace Demoulas.ProfitSharing.Common.Contracts.Response.Administration;

public sealed record TaxCodeAdminDto
{
    public required char Id { get; init; }

    public required string Name { get; init; }

    public bool IsAvailableForDistribution { get; init; }

    public bool IsAvailableForForfeiture { get; init; }

    public bool IsProtected { get; init; }

    public static TaxCodeAdminDto ResponseExample()
    {
        return new TaxCodeAdminDto
        {
            Id = '7',
            Name = "Normal distribution",
            IsAvailableForDistribution = true,
            IsAvailableForForfeiture = true,
            IsProtected = true
        };
    }
}
