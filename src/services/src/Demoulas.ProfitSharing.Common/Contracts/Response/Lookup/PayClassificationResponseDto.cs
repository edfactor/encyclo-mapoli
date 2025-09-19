using System.ComponentModel.DataAnnotations;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

public sealed record PayClassificationResponseDto : ILookupTable<string>
{
    [Key] // This key is used by the eager loading cache system to identify the unique values in an object
    public required string Id { get; set; }

    public required string Name { get; set; }
}
