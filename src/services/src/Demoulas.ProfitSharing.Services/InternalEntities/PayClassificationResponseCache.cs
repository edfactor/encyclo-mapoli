using System.ComponentModel.DataAnnotations;
using Demoulas.Common.Caching.Helpers;


namespace Demoulas.ProfitSharing.Services.InternalEntities;
public sealed record PayClassificationResponseCache : CacheDataObject
{
    [Key] // This key is used by the eager loading cache system to identify the unique values in an object
    public byte Id { get; set; }
    public required string Name { get; set; }
}
