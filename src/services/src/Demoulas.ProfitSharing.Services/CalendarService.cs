using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Services;
/// <summary>
/// Provides services related to calendar operations within the profit-sharing context.
/// </summary>
public sealed class CalendarService : ICalendarService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IAccountingPeriodsService _accountingPeriodsService;

    public CalendarService(IProfitSharingDataContextFactory dataContextFactory, IAccountingPeriodsService accountingPeriodsService)
    {
        _dataContextFactory = dataContextFactory;
        _accountingPeriodsService = accountingPeriodsService;
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
        return _dataContextFactory.UseReadOnlyContext(c => _accountingPeriodsService.FindWeekendingDateFromDate(c, dateTime, cancellationToken));
    }

    /// <summary>
    /// Retrieves the start and end accounting dates for a specified calendar year.
    /// </summary>
    /// <param name="calendarYear">The calendar year for which to retrieve the accounting dates.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the start and end accounting dates.</returns>
    public Task<CalendarResponseDto> GetYearStartAndEndAccountingDates(short calendarYear, CancellationToken cancellationToken = default)
    {
        return _dataContextFactory.UseReadOnlyContext(c => _accountingPeriodsService.GetYearStartAndEndAccountingDates(c, calendarYear, cancellationToken));
    }
}
