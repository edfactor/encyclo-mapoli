using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Services.Services.Reports;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY426;

public class Pay426Tests : PristineBaseTest
{
    private readonly ProfitSharingSummaryReportService _reportService;

    public Pay426Tests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _reportService = new ProfitSharingSummaryReportService(DbFactory, CalendarService, TotalService, DemographicReaderService, new NullLogger<ProfitSharingSummaryReportService>());
    }

    [Fact]
    public async Task Pay426Test()
    {
        // Get expected totals by parsing READY Report (full PAY426 main report format)
        string expectedReportText = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R08-PAY426").Trim();
        YearEndProfitSharingReportResponse expectedResponse = Pay426Parser.Parse(expectedReportText);

        // Get actual totals from SMART using GetYearEndProfitSharingTotalsAsync
        // This method applies PAY426 filtering: "ALL EMPLOYEES 1000 OR OVER AND 18 YEARS OR OLDER"
        BadgeNumberRequest smartRequest = new() { ProfitYear = 2025, UseFrozenData = false };

        YearEndProfitSharingReportTotals actualTotals = await _reportService.GetYearEndProfitSharingTotalsAsync(smartRequest, CancellationToken.None);

        // Compare totals
        actualTotals.NumberOfEmployees.ShouldBe(expectedResponse.NumberOfEmployees, "NumberOfEmployees should match");
        actualTotals.NumberOfNewEmployees.ShouldBe(expectedResponse.NumberOfNewEmployees, "NumberOfNewEmployees should match");
        actualTotals.NumberOfEmployeesUnder21.ShouldBe(expectedResponse.NumberOfEmployeesUnder21, "NumberOfEmployeesUnder21 should match");
        actualTotals.WagesTotal.ShouldBe(expectedResponse.WagesTotal, "WagesTotal should match");
        actualTotals.HoursTotal.ShouldBe(expectedResponse.HoursTotal, "HoursTotal should match");
        actualTotals.PointsTotal.ShouldBe(expectedResponse.PointsTotal, "PointsTotal should match");
    }
}
