using System.Diagnostics.CodeAnalysis;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Reports;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;

public class GetEligibilityIntegrationTests
{
    private readonly AccountingPeriodsService _aps = new();
    private readonly CalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dbFactory;
    private readonly ITestOutputHelper _testOutputHelper;

    public GetEligibilityIntegrationTests(ITestOutputHelper testOutputHelper)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().AddUserSecrets<ProfitShareUpdateTests>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing"]!;
        _dbFactory = new PristineDataContextFactory(connectionString, true);
        _calendarService = new CalendarService(_dbFactory, _aps);
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task BasicTest()
    {
        GetEligibleEmployeesService es = new(_dbFactory, _calendarService);
        GetEligibleEmployeesResponse empls = await es.GetEligibleEmployeesAsync(new ProfitYearRequest { ProfitYear = 2024, Take = int.MaxValue }, CancellationToken.None);

        _testOutputHelper.WriteLine("On Frozen: " + empls.NumberReadOnFrozen);
        _testOutputHelper.WriteLine("Not Selected: " + empls.NumberNotSelected);
        _testOutputHelper.WriteLine("Written: " + empls.NumberWritten);
        _testOutputHelper.WriteLine($"Got {empls.Response.Results.Count()} employees");
        empls.NumberReadOnFrozen.Should().BePositive();
        empls.NumberNotSelected.Should().BePositive();
        empls.NumberWritten.Should().BePositive();
    }
}
