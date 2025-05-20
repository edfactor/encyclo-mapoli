using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.ItOperations;
using Demoulas.ProfitSharing.Services.Reports;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;

public class GetEligibilityIntegrationTests : PristineBaseTest
{
    public GetEligibilityIntegrationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }
    

    [Fact]
    public async Task BasicTest()
    {
        GetEligibleEmployeesService es = new(DbFactory, CalendarService, new DemographicReaderService(new FrozenService(DbFactory), new HttpContextAccessor()));
        GetEligibleEmployeesResponse empls = await es.GetEligibleEmployeesAsync(new ProfitYearRequest { ProfitYear = 2024, Take = int.MaxValue }, CancellationToken.None);

        TestOutputHelper.WriteLine("On Frozen: " + empls.NumberReadOnFrozen);
        TestOutputHelper.WriteLine("Not Selected: " + empls.NumberNotSelected);
        TestOutputHelper.WriteLine("Written: " + empls.NumberWritten);
        TestOutputHelper.WriteLine($"Got {empls.Response.Results.Count()} employees");
        empls.Response.Results.Count().Should().BePositive();
        empls.NumberReadOnFrozen.Should().BePositive();
        empls.NumberNotSelected.Should().BePositive();
        empls.NumberWritten.Should().BePositive();
    }
}
