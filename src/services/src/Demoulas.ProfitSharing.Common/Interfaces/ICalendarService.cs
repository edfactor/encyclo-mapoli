using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface ICalendarService
{
    /// <summary>
    /// Finds the week-ending date from a given date.
    /// </summary>
    /// <param name="dateTime">The date from which to find the week-ending date.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the week-ending date.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the <paramref name="dateTime"/> is not within the valid range (between January 1, 2000, and 5 years from today's date).
    /// </exception>
    Task<DateOnly> FindWeekendingDateFromDateAsync(DateOnly dateTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the start and end accounting dates for a specified calendar year.
    /// </summary>
    /// <param name="calendarYear">The calendar year for which to retrieve the accounting dates.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the start and end accounting dates.</returns>
    Task<CalendarResponseDto> GetYearStartAndEndAccountingDatesAsync(short calendarYear, CancellationToken cancellationToken = default);
}
