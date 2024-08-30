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

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class MilitaryAndRehireServiceTests : ApiTestBase<Demoulas.ProfitSharing.Api.Program>
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;


    public MilitaryAndRehireServiceTests()
    {
        _dataContextFactory = MockDbContextFactory;
    }


    [Fact(DisplayName = "Check Calendar can be accessed")]
    public async Task CheckCalendarAccess()
    {
        long count = await _dataContextFactory.UseReadOnlyContext(c => c.CaldarRecords.LongCountAsync());

        count.ShouldBeEquivalentTo(CaldarRecordSeeder.Records.Length);
    }

    [Fact(DisplayName = "Find Weekending Date")]
    public async Task FindWeekendingDate()
    {
        var calendarService = ServiceProvider?.GetRequiredService<CalendarService>()!;

        var date = DateTime.Today.AddYears(-5).ToDateOnly();
        var weekEndingDate =await calendarService.FindWeekendingDateFromDate(date);

        weekEndingDate.Should().BeOnOrAfter(date);
        weekEndingDate.Should().HaveDay(7);
    }

}
