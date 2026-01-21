using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Services.Services.Reports;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;

public class GetEligibilityIntegrationTests : PristineBaseTest
{
    public GetEligibilityIntegrationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task BasicTest()
    {
        GetEligibleEmployeesService es = new(DbFactory, CalendarService, DemographicReaderService);
        GetEligibleEmployeesResponse empls = await es.GetEligibleEmployeesAsync(new ProfitYearRequest { ProfitYear = 2025, Take = int.MaxValue }, CancellationToken.None);

        TestOutputHelper.WriteLine("On Frozen: " + empls.NumberReadOnFrozen);
        TestOutputHelper.WriteLine("Not Selected: " + empls.NumberNotSelected);
        TestOutputHelper.WriteLine("Written: " + empls.NumberWritten);
        TestOutputHelper.WriteLine($"Got {empls.Response.Results.Count()} employees");
        empls.Response.Results.Count().ShouldBeGreaterThan(0);
        empls.NumberReadOnFrozen.ShouldBeGreaterThan(0);
        empls.NumberNotSelected.ShouldBeGreaterThan(0);
        empls.NumberWritten.ShouldBeGreaterThan(0);
    }
}
