using System.Globalization;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.NotOwned;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using FastEndpoints;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests;

public class CalendarServiceTests : ApiTestBase<Program>
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;


    public CalendarServiceTests()
    {
        _dataContextFactory = MockDbContextFactory;
    }


    [Fact(DisplayName = "Check Calendar can be accessed")]
    public async Task CheckCalendarAccess()
    {
        long count = await _dataContextFactory.UseReadOnlyContext(c => c.AccountingPeriods.LongCountAsync(CancellationToken.None));

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
        var calendarService = ServiceProvider?.GetRequiredService<ICalendarService>()!;

        var weekEndingDate = await calendarService.FindWeekendingDateFromDateAsync(date);

        weekEndingDate.Should().BeOnOrAfter(date);

        // Verify that the weekEndingDate is a Saturday
        weekEndingDate.DayOfWeek.Should().Be(DayOfWeek.Saturday);
    }

    [Fact(DisplayName = "Find Weekending Date - Invalid Date")]
    public Task FindWeekendingDate_InvalidDate()
    {
        var invalidDate = DateOnly.MaxValue;
        var calendarService = ServiceProvider?.GetRequiredService<ICalendarService>()!;
        Func<Task> act = async () => await calendarService.FindWeekendingDateFromDateAsync(invalidDate);
        return act.Should().ThrowAsync<Exception>();
    }


    [Fact(DisplayName = "Find Weekending Date - Future Date")]
    public Task FindWeekendingDate_FutureDate()
    {
        var futureDate = DateOnly.FromDateTime(DateTime.Now.AddYears(6));
        var calendarService = ServiceProvider?.GetRequiredService<ICalendarService>()!;
        Func<Task> act = async () => await calendarService.FindWeekendingDateFromDateAsync(futureDate);
        return act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithMessage($"{AccountingPeriodsService.InvalidDateError} (Parameter 'dateTime')");
    }


    [Fact(DisplayName = "Find Weekending Date - Valid Date")]
    public async Task FindWeekendingDate_ValidDate()
    {
        var validDate = DateOnly.ParseExact("230101", "yyMMdd", CultureInfo.InvariantCulture);
        var calendarService = ServiceProvider?.GetRequiredService<ICalendarService>()!;
        var weekEndingDate = await calendarService.FindWeekendingDateFromDateAsync(validDate);
        weekEndingDate.Should().BeOnOrAfter(validDate);
        weekEndingDate.DayOfWeek.Should().Be(DayOfWeek.Saturday);
    }

    [Fact(DisplayName = "PS-366 Get start and end dates for a provided fiscal year")]
    public async Task GetStartEndProvidedFiscalYear()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        TestResult<CalendarResponseDto> response =
            await ApiClient
                .GETAsync<CalendarRecordEndpoint, CalendarRequestDto, CalendarResponseDto>(new CalendarRequestDto { ProfitYear = 2023 });

        response.Result.Should().NotBeNull();
        response.Result.FiscalBeginDate.Should().NotBeOnOrBefore(DateOnly.MinValue);
        response.Result.FiscalEndDate.Should().NotBeOnOrBefore(DateOnly.MinValue);
    }
}
