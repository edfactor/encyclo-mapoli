using System.ComponentModel.DataAnnotations;
using Demoulas.Common.Contracts.Caching;


namespace Demoulas.ProfitSharing.Services.InternalEntities;
public sealed record StoreInfoCache : CacheDataObject
{
    [Key] // This key is used by the eager loading cache system to identify the unique values in an object
    public required short StoreId { get; set; }
}
