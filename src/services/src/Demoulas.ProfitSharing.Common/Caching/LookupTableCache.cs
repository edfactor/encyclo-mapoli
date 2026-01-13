using System.ComponentModel.DataAnnotations;
using Demoulas.Common.Contracts.Caching;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Caching;

public sealed record LookupTableCache<TType> : CacheDataObject, ILookupTable<TType>
{
    [Key] // This key is used by the eager loading cache system to identify the unique values in an object
    public required TType Id { get; set; }
    public required string Name { get; set; }
}
