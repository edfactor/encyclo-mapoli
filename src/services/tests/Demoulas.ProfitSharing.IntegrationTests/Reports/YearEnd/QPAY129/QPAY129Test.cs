using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services.Reports;
using Microsoft.Extensions.Hosting;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.QPAY129;

public class QPAY129Test : PristineBaseTest
{
    private readonly ICleanupReportService _cleanupReportService;

    public QPAY129Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        TestLoggerFactory loggerFactory = new(testOutputHelper);
        _cleanupReportService = new CleanupReportService(
            DbFactory,
            loggerFactory,
            CalendarService,
            TotalService,
            new Mock<IHostEnvironment>().Object,
            DemographicReaderService);
    }

    [Fact]
    public async Task TestQPAY129()
    {
        // Arrange - Parse expected totals from READY report
        string expectedReport = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R4-QPAY129").Trim();
        DistributionsAndForfeitureTotalsResponse expectedTotals = ReadyReportParser.ParseTotals(expectedReport);

        // Act - Get actual results from service
        DistributionsAndForfeituresRequest request = new() { StartDate = new DateOnly(2025, 1, 1), EndDate = new DateOnly(2025, 12, 31) };

        DistributionsAndForfeitureTotalsResponse actualTotals = (await _cleanupReportService.GetDistributionsAndForfeitureAsync(request, CancellationToken.None)).Value!;

        // Assert - Compare the 4 main totals in one line
        (actualTotals.DistributionTotal, actualTotals.FederalTaxTotal, actualTotals.StateTaxTotal, actualTotals.ForfeitureTotal)
            .ShouldBe((expectedTotals.DistributionTotal, expectedTotals.FederalTaxTotal, expectedTotals.StateTaxTotal, expectedTotals.ForfeitureTotal));
    }
}
