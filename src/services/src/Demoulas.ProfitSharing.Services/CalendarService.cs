using System.Text.Json;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Demoulas.ProfitSharing.Services;
/// <summary>
/// Hosted service for calendar operations, periodically refreshing calendar data in distributed cache.
/// </summary>
public sealed class CalendarService : ICalendarService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IAccountingPeriodsService _accountingPeriodsService;
    private readonly IDistributedCache _distributedCache;
    private readonly TimeSpan _refreshInterval = TimeSpan.FromHours(4); // Every four hours refresh
    private const string YearDatesCacheKey = "CalendarService_YearDates";

    public CalendarService(IProfitSharingDataContextFactory dataContextFactory, IAccountingPeriodsService accountingPeriodsService, IDistributedCache distributedCache)
    {
        _dataContextFactory = dataContextFactory;
        _accountingPeriodsService = accountingPeriodsService;
        _distributedCache = distributedCache;
    }

    public Task<DateOnly> FindWeekendingDateFromDateAsync(DateOnly dateTime, CancellationToken cancellationToken = default)
    {
        // No caching for weekending date, but could be added if needed
        return _dataContextFactory.UseWarehouseContext(c => _accountingPeriodsService.FindWeekendingDateFromDateAsync(c, dateTime, cancellationToken));
    }

    public async Task<CalendarResponseDto> GetYearStartAndEndAccountingDatesAsync(short calendarYear, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{YearDatesCacheKey}_{calendarYear}";
        var cached = await _distributedCache.GetAsync(cacheKey, cancellationToken);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<CalendarResponseDto>(cached)!;
        }

        short minCalendarYear = (short)ReferenceData.DsmMinValue.Year;
        if (calendarYear < minCalendarYear)
        {
            throw new ArgumentOutOfRangeException(nameof(calendarYear),
                $"Calendar Year value must be greater than {ReferenceData.DsmMinValue.Year}");
        }

        var returnValue = await _dataContextFactory.UseWarehouseContext(async ctx =>
        {
            var fiscalDates = await _accountingPeriodsService.GetFiscalYearDatesAsync(ctx, calendarYear, cancellationToken);

            return new CalendarResponseDto { FiscalBeginDate = fiscalDates.StartDate, FiscalEndDate = fiscalDates.EndDate };
        });

        var serialized = JsonSerializer.SerializeToUtf8Bytes(returnValue);
        await _distributedCache.SetAsync(cacheKey, serialized, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _refreshInterval },
            cancellationToken);
        return returnValue;
    }
}
