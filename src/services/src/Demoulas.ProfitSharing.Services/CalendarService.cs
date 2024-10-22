using System.Data.SqlTypes;
using Demoulas.ProfitSharing.Data.Interfaces;
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
                .OrderBy(record => record.WeekEndingDate)
                .Select(r => r.WeekEndingDate)
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

        var startingDate = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            return await context.CaldarRecords
                .Where(r => r.WeekEndingDate >= new DateOnly(calendarYear, 1, 1) &&
                            r.WeekEndingDate <= new DateOnly(calendarYear, 12, 31))
                .Select(r => r.WeekEndingDate)
                .MinAsync(cancellationToken);
        });

        var endingDate = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            // Filter records where WeekEndingDate is in December of the given calendar year
            var decStart = new DateOnly(calendarYear, 12, 1);
            var decEnd = new DateOnly(calendarYear, 12, 31);
            var decemberRecords = context.CaldarRecords
                .Where(r => r.WeekEndingDate >= decStart && r.WeekEndingDate <= decEnd);

            // Get the maximum ACC_WEEKN for December
            var maxAccWeekn = await decemberRecords
                .MaxAsync(r => r.AccWeekN, cancellationToken);

            // Retrieve the WeekEndingDate for the record with ACC_PERIOD == 12 and the maximum ACC_WEEKN
            var endingWeekEndingDate = await decemberRecords
                .Where(r => r.AccPeriod == 12 && r.AccWeekN == maxAccWeekn)
                .Select(r => r.WeekEndingDate)
                .FirstOrDefaultAsync(cancellationToken);

            return endingWeekEndingDate;
        });

        if (endingDate == default)
        {
            endingDate = new DateOnly(calendarYear, 12, 31);
            while (endingDate.DayOfWeek != DayOfWeek.Saturday)
            {
                endingDate = endingDate.AddDays(1);
            }
        }

        return (BeginDate: startingDate.AddDays(1), YearEndDate: endingDate);
    }

}
