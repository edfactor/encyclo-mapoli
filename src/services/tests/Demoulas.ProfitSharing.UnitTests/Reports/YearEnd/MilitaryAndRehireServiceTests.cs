using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.NotOwned;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
}
