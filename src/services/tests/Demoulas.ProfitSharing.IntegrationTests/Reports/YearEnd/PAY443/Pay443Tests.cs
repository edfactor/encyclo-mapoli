using System.Reflection;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Reports;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY443;

public class Pay443Tests : PristineBaseTest
{
    private readonly IForfeituresAndPointsForYearService _forfeituresAndPointsForYearService;

    public Pay443Tests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _forfeituresAndPointsForYearService = new ForfeituresAndPointsForYearService(DbFactory, TotalService, DemographicReaderService, new Mock<IPayrollDuplicateSsnReportService>().Object);
    }

    [Fact]
    public async Task TestPay443()
    {
        // Get Response by parsing READY Report
        string expectedReport = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R20-PAY443").Trim();

        ForfeituresAndPointsForYearResponseWithTotals expectedResponse = ReadyReportParser.ParseReport(expectedReport);

        // Get a hot response from SMART
        ForfeituresAndPointsForYearResponseWithTotals actualResponse =
            await _forfeituresAndPointsForYearService.GetForfeituresAndPointsForYearAsync(new FrozenProfitYearRequest { ProfitYear = 2024, Take = int.MaxValue });

        // We do not test that the order is identical.
        HashSet<ForfeituresAndPointsForYearResponse> actualRows = actualResponse.Response.Results.ToHashSet();
        HashSet<ForfeituresAndPointsForYearResponse> expectedRows = expectedResponse.Response.Results.ToHashSet();

        // READY does not provide IsExecutive, so we black it out in this test.
        actualRows = actualRows.Select(ar =>
        {
            ar.IsExecutive = false;
            return ar;
        }).ToHashSet();

        IEnumerable<ForfeituresAndPointsForYearResponse> smartOnly = actualRows.Except(expectedRows);
        IEnumerable<ForfeituresAndPointsForYearResponse> readyOnly = expectedRows.Except(actualRows);
        if (smartOnly.Any())
        {
            TestOutputHelper.WriteLine($"SMART Only  count: {smartOnly.Count()}");
            foreach (ForfeituresAndPointsForYearResponse s in smartOnly.OrderBy(s => s.BadgeNumber).Take(5))
            {
                TestOutputHelper.WriteLine(s.ToString());
            }
        }

        if (readyOnly.Any())
        {
            TestOutputHelper.WriteLine($"READY Only  count: {readyOnly.Count()}");
            foreach (ForfeituresAndPointsForYearResponse r in readyOnly.OrderBy(s => s.BadgeNumber).Take(5))
            {
                TestOutputHelper.WriteLine(r.ToString());
            }
        }

        smartOnly.Count().ShouldBe(0);
        readyOnly.Count().ShouldBe(0);

        actualRows.Count.ShouldBe(expectedRows.Count);
        actualRows.SetEquals(expectedRows).ShouldBeTrue();

        // ignore the report time
        expectedResponse = expectedResponse with
        {
            ReportDate = actualResponse.ReportDate,
            Response = new PaginatedResponseDto<ForfeituresAndPointsForYearResponse> { Total = 0, Results = [] }
        };
        actualResponse = actualResponse with { Response = new PaginatedResponseDto<ForfeituresAndPointsForYearResponse> { Total = 0, Results = [] } };

        actualResponse.ShouldBeEquivalentTo(expectedResponse);
    }

}
