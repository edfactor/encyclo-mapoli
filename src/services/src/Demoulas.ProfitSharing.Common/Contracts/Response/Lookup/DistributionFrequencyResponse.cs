using System.ComponentModel.DataAnnotations;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

public sealed record DistributionFrequencyResponse
{
    [Key]
    public char Id { get; set; }

    public required string Name { get; set; }

    public static DistributionFrequencyResponse ResponseExample()
    {
        return new DistributionFrequencyResponse
        {
            Id = 'W',
            Name = "Weekly"
        };
    }
}
