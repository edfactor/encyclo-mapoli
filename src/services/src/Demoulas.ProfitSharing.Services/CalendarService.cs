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
        if (dateTime < DateOnly.FromDateTime(new DateTime(2000, 1, 1)) || dateTime > DateOnly.FromDateTime(DateTime.Today.AddYears(5)))
#pragma warning restore S6562
        {
            throw new ArgumentOutOfRangeException(nameof(dateTime), InvalidDateError);
        }
        return _dataContextFactory.UseReadOnlyContext(context =>
        {
            return context.CaldarRecords.Where(record => record.WeekDate > dateTime)
                .Select(r => r.AccApWkend).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        });
    }
}
