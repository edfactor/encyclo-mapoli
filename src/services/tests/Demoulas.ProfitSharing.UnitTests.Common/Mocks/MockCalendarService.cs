using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Common.Mocks;

/// <summary>
/// Mock calendar service for unit tests that returns valid fiscal dates
/// for any requested year, avoiding issues when calendar data doesn't exist
/// (e.g., when DateTime.Now.Year is 2026 but calendar data only goes through 2025).
/// </summary>
public static class MockCalendarService
{
    /// <summary>
    /// Creates a mock ICalendarService that returns valid fiscal dates for any year.
    /// </summary>
    public static ICalendarService Initialize()
    {
        var mock = new Mock<ICalendarService>();

        // Mock GetYearStartAndEndAccountingDatesAsync to return valid fiscal dates for any year
        mock.Setup(m => m.GetYearStartAndEndAccountingDatesAsync(It.IsAny<short>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((short calendarYear, CancellationToken _) => new CalendarResponseDto
            {
                // Fiscal year starts on the last Sunday of the previous calendar year
                // and ends on the last Saturday of the current calendar year
                FiscalBeginDate = GetLastSundayOfYear((short)(calendarYear - 1)),
                FiscalEndDate = GetLastSaturdayOfYear(calendarYear)
            });

        // Mock FindWeekendingDateFromDateAsync to return the next Saturday on or after the given date
        mock.Setup(m => m.FindWeekendingDateFromDateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DateOnly date, CancellationToken _) => GetNextSaturday(date));

        return mock.Object;
    }

    private static DateOnly GetLastSundayOfYear(short year)
    {
        var date = new DateOnly(year, 12, 31);
        while (date.DayOfWeek != DayOfWeek.Sunday)
        {
            date = date.AddDays(-1);
        }
        return date;
    }

    private static DateOnly GetLastSaturdayOfYear(short year)
    {
        var date = new DateOnly(year, 12, 31);
        while (date.DayOfWeek != DayOfWeek.Saturday)
        {
            date = date.AddDays(-1);
        }
        return date;
    }

    private static DateOnly GetNextSaturday(DateOnly date)
    {
        while (date.DayOfWeek != DayOfWeek.Saturday)
        {
            date = date.AddDays(1);
        }
        return date;
    }
}
