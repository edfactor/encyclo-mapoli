using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services.Reports;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY443;

public class Pay443Tests : PristineBaseTest
{
    private readonly IForfeituresAndPointsForYearService _forfeituresAndPointsForYearService;

    public Pay443Tests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _forfeituresAndPointsForYearService = new ForfeituresAndPointsForYearService(DbFactory, TotalService, DemographicReaderService, new Mock<IPayrollDuplicateSsnReportService>().Object, new Mock<ICrossReferenceValidationService>().Object);
    }

    [Fact]
    public async Task TestPay443()
    {
        // Get Response by parsing READY Report
        string expectedReport = ReadEmbeddedResource(".golden.R20-PAY443").Trim();

        ForfeituresAndPointsForYearResponseWithTotals expectedResponse = ReadyReportParser.ParseReport(expectedReport);

        // Get a hot response from SMART
        ForfeituresAndPointsForYearResponseWithTotals actualResponse =
            await _forfeituresAndPointsForYearService.GetForfeituresAndPointsForYearAsync(new FrozenProfitYearRequest { ProfitYear = TestConstants.OpenProfitYear, Take = int.MaxValue });

        // READY uses the name without initial, so we remove the middle initial from the name in smart.
        actualResponse = actualResponse with
        {
            Response = actualResponse.Response with
            {
                Results = actualResponse.Response.Results.Select(r => r with
                {
                    EmployeeName = RemoveMiddleInitial(r.EmployeeName)
                }).ToList()
            }
        };

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

    public static string? RemoveMiddleInitial(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        // Names are in format "LAST, FIRST MIDDLE" - we want to remove the middle initial
        // Example: "SMITH, JOHN K" becomes "SMITH, JOHN"
        var parts = name.Split(',');
        if (parts.Length != 2)
        {
            return name;
        }

        var lastName = parts[0].Trim();
        var firstAndMiddle = parts[1].Trim();

        // Split the first and middle parts
        var nameTokens = firstAndMiddle.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return nameTokens.Length <= 1 ? name :
            // Keep only the first name, remove middle initial(s)
            $"{lastName}, {nameTokens[0]}";
    }

}
