using System.Text.Json;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
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
        return _dataContextFactory.UseReadOnlyContext(c => _accountingPeriodsService.FindWeekendingDateFromDateAsync(c, dateTime, cancellationToken), cancellationToken);
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

        var returnValue = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var fiscalDates = await ctx.AccountingPeriods
                .Where(r => r.WeekendingDate >= new DateOnly(calendarYear, 1, 1) &&
                            (r.WeekNo == 1 || r.WeekNo >= 52)).GroupBy(r => 1) // Group all records to fetch min and max in one query
                .Select(g => new
                {
                    StartingDate = g.Min(r => r.WeekendingDate).AddDays(-6),
                    EndingDate = g.Where(r => r.Period == 12)
                        .OrderByDescending(r => r.WeekNo)
                        .Select(r => r.WeekendingDate)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

            if (fiscalDates == null || fiscalDates.EndingDate == default || fiscalDates.StartingDate == default)
            {
                var startingDate = fiscalDates?.StartingDate ?? DateOnly.MinValue;
                var endingDate = fiscalDates?.EndingDate ?? DateOnly.MinValue;

                // Adjust startingDate to the first Sunday on or after the date if it's null or MinValue
                if (startingDate == DateOnly.MinValue || startingDate == default)
                {
                    startingDate = AdjustToNextSunday(new DateOnly(calendarYear, 01, 01));
                }

                // Adjust endingDate to the first Saturday on or after the date if it's null or MinValue
                if (endingDate == DateOnly.MinValue || endingDate == default)
                {
                    endingDate = AdjustToNextSaturday(new DateOnly(calendarYear, 12, 31));
                }

                fiscalDates = new { StartingDate = startingDate, EndingDate = endingDate };
            }

            return new CalendarResponseDto { FiscalBeginDate = fiscalDates.StartingDate, FiscalEndDate = fiscalDates.EndingDate };
        }, cancellationToken);

        var serialized = JsonSerializer.SerializeToUtf8Bytes(returnValue);
        await _distributedCache.SetAsync(cacheKey, serialized, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _refreshInterval },
            cancellationToken);
        return returnValue;
    }

    private static DateOnly AdjustToNextSunday(DateOnly date)
    {
        while (date.DayOfWeek != DayOfWeek.Sunday)
        {
            date = date.AddDays(1);
        }
        return date;
    }

    private static DateOnly AdjustToNextSaturday(DateOnly date)
    {
        while (date.DayOfWeek != DayOfWeek.Saturday)
        {
            date = date.AddDays(1);
        }
        return date;
    }
}
