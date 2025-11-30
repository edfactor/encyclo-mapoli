using System.Diagnostics.CodeAnalysis;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY426;

/**
 * Sanity Check the report from READY that the total hours and wages match the line items
 */
[SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
public class Pay426ParserTests : PristineBaseTest
{
    public Pay426ParserTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void Pay426FullTest()
    {
        string fullReportText = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R8-PAY426").Trim();

        // Parse will validate internally and throw if validation fails
        YearEndProfitSharingReportResponse response = Pay426Parser.Parse(fullReportText);

        // Verify the response structure is populated
        response.ShouldNotBeNull();
        response.Response.ShouldNotBeNull();
        response.Response.Results.ShouldNotBeNull();

        // Verify employee detail records match totals
        response.Response.Total.ShouldBe((int)response.NumberOfEmployees, "Response.Total should match NumberOfEmployees");
        response.Response.Results.Count().ShouldBe((int)response.NumberOfEmployees, "Results count should match NumberOfEmployees");

        // Finally - check hard coded reference values 

        // Verify expected totals (these are the known values from Today's R8-PAY426 report)
        response.WagesTotal.ShouldBe(211_191_778.18m, "WagesTotal should match SECTION TOTAL");
        response.HoursTotal.ShouldBe(7_732_647m, "HoursTotal should match SECTION TOTAL");
        response.PointsTotal.ShouldBe(2_111_925m, "PointsTotal should match SECTION TOTAL");
        response.NumberOfEmployees.ShouldBe(4939, "NumberOfEmployees should match ALL-EMP");
        response.NumberOfNewEmployees.ShouldBe(62, "NumberOfNewEmployees should match NEW-EMP");
        response.NumberOfEmployeesUnder21.ShouldBe(205, "NumberOfEmployeesUnder21 should match EMP<21");
        response.NumberOfEmployeesInPlan.ShouldBe(4672, "NumberOfEmployeesInPlan should match IN-PLAN");
    }
}
