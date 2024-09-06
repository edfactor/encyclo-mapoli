using System.Globalization;
using System.Net;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.NotOwned;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using Demoulas.Util.Extensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.UnitTests;

public class CalendarServiceTests : ApiTestBase<Api.Program>
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;


    public CalendarServiceTests()
    {
        _dataContextFactory = MockDbContextFactory;
    }


    [Fact(DisplayName = "Check Calendar can be accessed")]
    public async Task CheckCalendarAccess()
    {
        long count = await _dataContextFactory.UseReadOnlyContext(c => c.CaldarRecords.LongCountAsync());

        count.ShouldBeEquivalentTo(CaldarRecordSeeder.Records.Length);
    }

    [InlineData("000101")]
    [InlineData("000203")]
    [InlineData("020509")]
    [InlineData("081009")]
    [InlineData("221009")]
    [Theory(DisplayName = "Find Weekending Date")]
    public async Task FindWeekendingDate(string sDate)
    {
        var date = DateOnly.ParseExact(sDate, "yyMMdd", CultureInfo.InvariantCulture);
        var calendarService = ServiceProvider?.GetRequiredService<CalendarService>()!;

        var weekEndingDate = await calendarService.FindWeekendingDateFromDate(date);

        weekEndingDate.Should().BeOnOrAfter(date);

        // Verify that the weekEndingDate is a Saturday
        weekEndingDate.DayOfWeek.Should().Be(DayOfWeek.Saturday);
    }

    [Fact(DisplayName = "Find Weekending Date - Invalid Date")]
    public async Task FindWeekendingDate_InvalidDate()
    {
        var invalidDate = DateOnly.MaxValue;
        var calendarService = ServiceProvider?.GetRequiredService<CalendarService>()!;
        Func<Task> act = async () => await calendarService.FindWeekendingDateFromDate(invalidDate);
        await act.Should().ThrowAsync<Exception>();
    }


    [Fact(DisplayName = "Find Weekending Date - Future Date")]
    public async Task FindWeekendingDate_FutureDate()
    {
        var futureDate = DateOnly.FromDateTime(DateTime.Now.AddYears(6));
        var calendarService = ServiceProvider?.GetRequiredService<CalendarService>()!;
        Func<Task> act = async () => await calendarService.FindWeekendingDateFromDate(futureDate);
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithMessage($"{CalendarService.InvalidDateError} (Parameter 'dateTime')");
    }


    [Fact(DisplayName = "Find Weekending Date - Valid Date")]
    public async Task FindWeekendingDate_ValidDate()
    {
        var validDate = DateOnly.ParseExact("230101", "yyMMdd", CultureInfo.InvariantCulture);
        var calendarService = ServiceProvider?.GetRequiredService<CalendarService>()!;
        var weekEndingDate = await calendarService.FindWeekendingDateFromDate(validDate);
        weekEndingDate.Should().BeOnOrAfter(validDate);
        weekEndingDate.DayOfWeek.Should().Be(DayOfWeek.Saturday);
    }


}
