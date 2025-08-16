using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services.Reports;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY443;

public class Pay443Tests : PristineBaseTest
{
    private readonly IForfeituresAndPointsForYearService _forfeituresAndPointsForYearService;

    public Pay443Tests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _forfeituresAndPointsForYearService = new ForfeituresAndPointsForYearService(DbFactory, TotalService, 
            DemographicReaderService);
    }

    [Fact]
    public async Task TestPay443()
    {
        // Get Response by parsing READY Report
        ForfeituresAndPointsForYearResponseWithTotals expectedResponse = ReadyReportParser.ParseReport();

        // Get a hot response from SMART
        ForfeituresAndPointsForYearResponseWithTotals actualResponse =
            await _forfeituresAndPointsForYearService.GetForfeituresAndPointsForYearAsync(new FrozenProfitYearRequest { ProfitYear = 2024, Take = int.MaxValue });

        // We do not test that the order is identical.
        HashSet<ForfeituresAndPointsForYearResponse> actualRows = actualResponse.Response.Results.ToHashSet();
        HashSet<ForfeituresAndPointsForYearResponse> expectedRows = expectedResponse.Response.Results.ToHashSet();
        actualRows.SetEquals(expectedRows).ShouldBeTrue();

        // ignore the report time
        expectedResponse = expectedResponse with
        {
            ReportDate = actualResponse.ReportDate, Response = new PaginatedResponseDto<ForfeituresAndPointsForYearResponse> { Total = 0, Results = [] }
        };
        actualResponse = actualResponse with { Response = new PaginatedResponseDto<ForfeituresAndPointsForYearResponse> { Total = 0, Results = [] } };

        actualResponse.ShouldBeEquivalentTo(expectedResponse);
    }
}
