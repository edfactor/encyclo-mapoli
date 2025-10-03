using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.UpdateSummary;

// Validate parser of lines of pay450 report.  Note output has negative signs at the end. 
public class ReportParserTests
{
    // Self test.
    [Fact]
    public void CheckParsing()
    {
        var record = ReportParser.ParseLine(
            "0705228 016 ROBINSON, CHARLIE             1,071,391.53-   4,071,393.55-    38      2        3,174,190.53-    2,174,190.53-    39      7");
        record.BadgeAndStore.ShouldBe("0705228 016");
        record.Name.ShouldBe("ROBINSON, CHARLIE");

        record.BeforeAmount.ShouldBe(-1071391.53m);
        record.BeforeVested.ShouldBe(-4071393.55m);
        record.BeforeYears.ShouldBe(38);
        record.BeforeEnroll.ShouldBe(2);

        record.AfterAmount.ShouldBe(-3174190.53m);
        record.AfterVested.ShouldBe(-2174190.53m);
        record.AfterYears.ShouldBe(39);
        record.AfterEnroll.ShouldBe(7);
    }
}
