using System.ComponentModel.DataAnnotations;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

public sealed record DistributionStatusResponse
{
    [Key]
    public char Id { get; set; }
    public required string Name { get; set; }

    public static DistributionStatusResponse ResponseExample()
    {
        return new DistributionStatusResponse
        {
            Id = 'A',
            Name = "Active"
        };
    }
}
