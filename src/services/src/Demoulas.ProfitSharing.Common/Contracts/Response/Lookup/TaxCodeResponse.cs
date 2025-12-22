using System.ComponentModel.DataAnnotations;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

public sealed record TaxCodeResponse
{
    [Key]
    public char Id { get; set; }
    public required string Name { get; set; }

    public static TaxCodeResponse ResponseExample()
    {
        return new TaxCodeResponse
        {
            Id = 'F',
            Name = "Federal"
        };
    }
}
