using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;
/// <summary>
/// Provides services related to calendar operations within the profit-sharing context.
/// </summary>
public sealed class CalendarService
{
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
    /// <remarks>
    /// The week-ending date is determined by querying the calendar records in the database.
    /// </remarks>
    public Task<DateOnly> FindWeekendingDateFromDate(DateOnly dateTime, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(context =>
        {
            return context.CaldarRecords.Where(record => record.WeekDate > dateTime)
                .Select(r => r.AccApWkend).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        });
    }
}
