using System.ComponentModel.DataAnnotations;
using Demoulas.Common.Contracts.Caching;
using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record DuplicateNamesAndBirthdaysCachedResponse : CacheDataObject
{
    [Key]
    public int CacheId => 1; // Single cached instance

    public required DateTimeOffset AsOfDate { get; init; }
    public required PaginatedResponseDto<DuplicateNamesAndBirthdaysResponse> Data { get; init; }
}
