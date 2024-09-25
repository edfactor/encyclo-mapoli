using System;
using System.Data.SqlTypes;
using Demoulas.ProfitSharing.Data.Interfaces;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;
/// <summary>
/// Provides services related to calendar operations within the profit-sharing context.
/// </summary>
public sealed class CalendarService
{
    public const string InvalidDateError = "The date must be between January 1, 2000, and 5 years from today's date.";
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public CalendarService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    /// <summary>
    /// Finds the week-ending date from a given date.
    /// </summary>
    /// <param name="dateTime">The date from which to find the week-ending date.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the week-ending date.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the <paramref name="dateTime"/> is not within the valid range (between January 1, 2000, and 5 years from today's date).
    /// </exception>
    public Task<DateOnly> FindWeekendingDateFromDate(DateOnly dateTime, CancellationToken cancellationToken = default)
    {
        // Validate the input date
#pragma warning disable S6562
        if (dateTime < new DateOnly(2000, 1, 1) || dateTime > DateOnly.FromDateTime(DateTime.Today.AddYears(5)))
#pragma warning restore S6562
        {
            throw new ArgumentOutOfRangeException(nameof(dateTime), InvalidDateError);
        }
        return _dataContextFactory.UseReadOnlyContext(context =>
        {
            return context.CaldarRecords.Where(record => record.WeekDate >= dateTime)
                .OrderBy(record => record.AccApWkend)
                .Select(r => r.AccApWkend)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        });
    }

    /// <summary>
    /// Retrieves the start and end accounting dates for a specified calendar year.
    /// </summary>
    /// <param name="calendarYear">The calendar year for which to retrieve the accounting dates.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the start and end accounting dates.</returns>
    public async Task<(DateOnly BeginDate, DateOnly YearEndDate)> GetYearStartAndEndAccountingDates(short calendarYear, CancellationToken cancellationToken = default)
    {
        if (calendarYear < SqlDateTime.MinValue.Value.Year || calendarYear > SqlDateTime.MaxValue.Value.Year)
        {
            throw new ArgumentOutOfRangeException(nameof(calendarYear), $"Calendar Year value must be between {SqlDateTime.MinValue.Value.Year} and {SqlDateTime.MaxValue.Value.Year}");
        }

        var calendarYearEnd = new DateOnly(calendarYear, 12, 31);

        var startingDate = await _dataContextFactory.UseReadOnlyContext(context =>
        {
            return context.CaldarRecords
                .Where(record => record.AccApWkend >= new DateOnly(calendarYear, 01, 01) &&
                                 record.AccApWkend <= calendarYearEnd)
                .Select(r => r.AccApWkend)
                .MinAsync(cancellationToken: cancellationToken);

        });

        var endingDate = await FindWeekendingDateFromDate(calendarYearEnd, cancellationToken);

        if (endingDate == DateOnly.MinValue)
        {
            endingDate = calendarYearEnd;
            // Loop until we find a Saturday
            while (endingDate.DayOfWeek != DayOfWeek.Saturday)
            {
                endingDate = endingDate.AddDays(1);
            }
        }

        return (BeginDate: startingDate.AddDays(1), YearEndDate: endingDate);
    }
}
