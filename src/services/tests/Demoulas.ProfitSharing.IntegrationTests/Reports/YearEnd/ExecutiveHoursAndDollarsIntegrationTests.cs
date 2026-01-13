using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Services.Reports;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;

public class ExecutiveHoursAndDollarsIntegrationTests : PristineBaseTest
{
    // Probably should define these somewhere more global, or be looked up dynamically 
    private const short YearThis = 2024;

    public ExecutiveHoursAndDollarsIntegrationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task Ensure_ServiceIsReturningValues()
    {
        // Act
        ExecutiveHoursAndDollarsRequest request = new() { ProfitYear = YearThis, HasExecutiveHoursAndDollars = true };
        ReportResponseBase<ExecutiveHoursAndDollarsResponse> response =
            await new ExecutiveHoursAndDollarsService(DbFactory, CalendarService).GetExecutiveHoursAndDollarsReportAsync(request, CancellationToken.None);

        response.Response.Results.Count().ShouldBeGreaterThan(0);
    }
}
