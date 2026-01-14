using Demoulas.Common.Caching;
using Demoulas.Common.Contracts.Caching;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Caching.HostedServices;

public sealed class DuplicateNamesAndBirthdaysHostedService : BaseCacheHostedService<DuplicateNamesAndBirthdaysCachedResponse>
{
    private readonly IDuplicateNamesAndBirthdaysService _duplicateNamesService;
    private readonly ILogger<DuplicateNamesAndBirthdaysHostedService> _logger;

    protected override string BaseKeyName => "DNAB";

    protected override ushort RefreshSeconds { get; set; } = (ushort)short.MaxValue; // Every 9.1 hours refresh

    public DuplicateNamesAndBirthdaysHostedService(
        IHostEnvironment hostEnvironment,
        IDistributedCache distributedCache,
        IDuplicateNamesAndBirthdaysService duplicateNamesService,
        ILogger<DuplicateNamesAndBirthdaysHostedService> logger)
        : base(hostEnvironment: hostEnvironment, distributedCache: distributedCache)
    {
        _duplicateNamesService = duplicateNamesService;
        _logger = logger;
    }

    public override Task<IEnumerable<DuplicateNamesAndBirthdaysCachedResponse>> GetDataToUpdateCacheAsync(CacheDataDictionary cdd, CancellationToken cancellation = default)
    {
        return GetDuplicateNamesAndBirthdaysData(cancellationToken: cancellation);
    }

    public override Task<IEnumerable<DuplicateNamesAndBirthdaysCachedResponse>> GetInitialDataToCacheAsync(CancellationToken cancellation = default)
    {
        return GetDuplicateNamesAndBirthdaysData(cancellationToken: cancellation);
    }

    private async Task<IEnumerable<DuplicateNamesAndBirthdaysCachedResponse>> GetDuplicateNamesAndBirthdaysData(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting to fetch duplicate names and birthdays data for cache");

            // Get the current calendar year
            var currentYear = (short)DateTime.UtcNow.Year;

            var request = new DuplicateNamesAndBirthdaysRequest
            {
                ProfitYear = currentYear
            };

            var reportData = await _duplicateNamesService.GetDuplicateNamesAndBirthdaysAsync(request, cancellationToken);

            var cachedResponse = new DuplicateNamesAndBirthdaysCachedResponse
            {
                AsOfDate = DateTimeOffset.UtcNow,
                Data = reportData.Response
            };

            _logger.LogInformation("Successfully fetched {Count} duplicate names and birthdays records for cache",
                reportData.Response.Total);

            return [cachedResponse];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch duplicate names and birthdays data for cache");
            return [];
        }
    }
}
