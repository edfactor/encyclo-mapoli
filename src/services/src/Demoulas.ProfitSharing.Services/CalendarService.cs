using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace Demoulas.ProfitSharing.Services;
/// <summary>
/// Hosted service for calendar operations, periodically refreshing calendar data in distributed cache.
/// </summary>
public sealed class CalendarService : BackgroundService, ICalendarService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IAccountingPeriodsService _accountingPeriodsService;
    private readonly IDistributedCache _distributedCache;
    private readonly TimeSpan _refreshInterval = TimeSpan.FromHours(2); // Every two hours refresh
    private const string YearDatesCacheKey = "CalendarService_YearDates";

    public CalendarService(IProfitSharingDataContextFactory dataContextFactory, IAccountingPeriodsService accountingPeriodsService, IDistributedCache distributedCache)
    {
        _dataContextFactory = dataContextFactory;
        _accountingPeriodsService = accountingPeriodsService;
        _distributedCache = distributedCache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RefreshCacheAsync(stoppingToken);
            await Task.Delay(_refreshInterval, stoppingToken);
        }
    }

    private async Task RefreshCacheAsync(CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var years = Enumerable.Range(now.Year - 1, 7).Select(y => (short)y);
        foreach (var year in years)
        {
            var yearDates = await _dataContextFactory.UseReadOnlyContext(c => _accountingPeriodsService.GetYearStartAndEndAccountingDatesAsync(c, year, cancellationToken));
            var serialized = JsonSerializer.SerializeToUtf8Bytes(yearDates);
            await _distributedCache.SetAsync($"{YearDatesCacheKey}_{year}", serialized, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _refreshInterval }, cancellationToken);
        }
    }

    public Task<DateOnly> FindWeekendingDateFromDateAsync(DateOnly dateTime, CancellationToken cancellationToken = default)
    {
        // No caching for weekending date, but could be added if needed
        return _dataContextFactory.UseReadOnlyContext(c => _accountingPeriodsService.FindWeekendingDateFromDateAsync(c, dateTime, cancellationToken));
    }

    public async Task<CalendarResponseDto> GetYearStartAndEndAccountingDatesAsync(short calendarYear, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{YearDatesCacheKey}_{calendarYear}";
        var cached = await _distributedCache.GetAsync(cacheKey, cancellationToken);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<CalendarResponseDto>(cached)!;
        }
        return await _dataContextFactory.UseReadOnlyContext(c => _accountingPeriodsService.GetYearStartAndEndAccountingDatesAsync(c, calendarYear, cancellationToken));
    }
}
