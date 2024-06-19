using System.ComponentModel.DataAnnotations;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public sealed record PayClassificationResponseDto
{
    [Key] // This key is used by the eager loading cache system to identify the unique values in an object
    public byte Id { get; set; }
    public required string Name { get; set; }
}
