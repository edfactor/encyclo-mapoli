using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY426N;

/// <summary>
///     Unit tests for Pay426N9Parser to verify correct parsing of R8-PAY426N-9 summary report format.
/// </summary>
public class Pay426N9ParserTests : PristineBaseTest
{
    public Pay426N9ParserTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    [Description("Pay426N9Parser should correctly parse all line items from R8-PAY426N-9 golden file")]
    public void Parse_ShouldExtractAllLineItemsCorrectly()
    {
        // Arrange - load the golden file
        string resourceName = "Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R8-PAY426N-9";
        string reportText = ReadEmbeddedResource(resourceName).Trim();

        // Act - parse the report
        YearEndProfitSharingReportSummaryResponse result = Pay426N9Parser.Parse(reportText);

        // Assert - verify we got all expected line items
        result.ShouldNotBeNull();
        result.LineItems.ShouldNotBeNull();
        result.LineItems.Count.ShouldBe(11, "Expected 11 line items (E, 1-8, X, N)");

        // Create dictionary for easy lookup by prefix
        var lineItemsByPrefix = result.LineItems.ToDictionary(li => li.LineItemPrefix);

        // Verify each line item matches the golden file values

        // E - ERROR REPORT EMPLOYEES
        var lineE = lineItemsByPrefix["E"];
        lineE.LineItemPrefix.ShouldBe("E");
        lineE.LineItemTitle.ShouldBe("ERROR REPORT EMPLOYEES");
        lineE.Subgroup.ShouldBe(string.Empty); // E is not in a subgroup
        lineE.NumberOfMembers.ShouldBe(0);
        lineE.TotalWages.ShouldBe(0.00m);
        lineE.TotalBalance.ShouldBe(0.00m);
        lineE.TotalHours.ShouldBeNull(); // Hours/points only for Line 2
        lineE.TotalPoints.ShouldBeNull();

        // 1 - AGE 18-20 WITH >= 1000 PS HOURS
        var line1 = lineItemsByPrefix["1"];
        line1.LineItemPrefix.ShouldBe("1");
        line1.LineItemTitle.ShouldBe("AGE 18-20 WITH >= 1000 PS HOURS");
        line1.Subgroup.ShouldBe("ACTIVE AND INACTIVE");
        line1.NumberOfMembers.ShouldBe(205);
        line1.TotalWages.ShouldBe(5_198_074.91m);
        line1.TotalBalance.ShouldBe(54_116.75m);
        line1.TotalHours.ShouldBeNull(); // Hours/points only for Line 2
        line1.TotalPoints.ShouldBeNull();

        // 2 - >= AGE 21 WITH >= 1000 PS HOURS (ONLY line with hours/points)
        var line2 = lineItemsByPrefix["2"];
        line2.LineItemPrefix.ShouldBe("2");
        line2.LineItemTitle.ShouldBe(">= AGE 21 WITH >= 1000 PS HOURS");
        line2.Subgroup.ShouldBe("ACTIVE AND INACTIVE");
        line2.NumberOfMembers.ShouldBe(4_734);
        line2.TotalWages.ShouldBe(211_191_778.18m);
        line2.TotalBalance.ShouldBe(506_124_990.05m);
        line2.TotalHours.ShouldBe(0); // Placeholder - actual hours from R8-PAY426-TOT
        line2.TotalPoints.ShouldBe(0); // Placeholder - actual points from R8-PAY426-TOT

        // 3 - < AGE 18
        var line3 = lineItemsByPrefix["3"];
        line3.LineItemPrefix.ShouldBe("3");
        line3.LineItemTitle.ShouldBe("< AGE 18");
        line3.Subgroup.ShouldBe("ACTIVE AND INACTIVE");
        line3.NumberOfMembers.ShouldBe(19);
        line3.TotalWages.ShouldBe(244_258.01m);
        line3.TotalBalance.ShouldBe(0.00m);
        line3.TotalHours.ShouldBeNull(); // Hours/points only for Line 2
        line3.TotalPoints.ShouldBeNull();

        // 4 - >= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT
        var line4 = lineItemsByPrefix["4"];
        line4.LineItemPrefix.ShouldBe("4");
        line4.LineItemTitle.ShouldBe(">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT");
        line4.Subgroup.ShouldBe("ACTIVE AND INACTIVE");
        line4.NumberOfMembers.ShouldBe(1_121);
        line4.TotalWages.ShouldBe(12_925_914.29m);
        line4.TotalBalance.ShouldBe(23_798_126.89m);
        line4.TotalHours.ShouldBeNull(); // Hours/points only for Line 2
        line4.TotalPoints.ShouldBeNull();

        // 5 - >= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT
        var line5 = lineItemsByPrefix["5"];
        line5.LineItemPrefix.ShouldBe("5");
        line5.LineItemTitle.ShouldBe(">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT");
        line5.Subgroup.ShouldBe("ACTIVE AND INACTIVE");
        line5.NumberOfMembers.ShouldBe(291);
        line5.TotalWages.ShouldBe(3_039_600.49m);
        line5.TotalBalance.ShouldBe(0.00m);
        line5.TotalHours.ShouldBeNull(); // Hours/points only for Line 2
        line5.TotalPoints.ShouldBeNull();

        // 6 - >= AGE 18 WITH >= 1000 PS HOURS (Terminated)
        var line6 = lineItemsByPrefix["6"];
        line6.LineItemPrefix.ShouldBe("6");
        line6.LineItemTitle.ShouldBe(">= AGE 18 WITH >= 1000 PS HOURS");
        line6.Subgroup.ShouldBe("TERMINATED");
        line6.NumberOfMembers.ShouldBe(141);
        line6.TotalWages.ShouldBe(4_594_615.39m);
        line6.TotalBalance.ShouldBe(10_928_291.19m);
        line6.TotalHours.ShouldBeNull(); // Hours/points only for Line 2
        line6.TotalPoints.ShouldBeNull();

        // 7 - >= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT (Terminated)
        var line7 = lineItemsByPrefix["7"];
        line7.LineItemPrefix.ShouldBe("7");
        line7.LineItemTitle.ShouldBe(">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT");
        line7.Subgroup.ShouldBe("TERMINATED");
        line7.NumberOfMembers.ShouldBe(108);
        line7.TotalWages.ShouldBe(616_836.02m);
        line7.TotalBalance.ShouldBe(0.00m);
        line7.TotalHours.ShouldBeNull(); // Hours/points only for Line 2
        line7.TotalPoints.ShouldBeNull();

        // 8 - >= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT (Terminated)
        var line8 = lineItemsByPrefix["8"];
        line8.LineItemPrefix.ShouldBe("8");
        line8.LineItemTitle.ShouldBe(">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT");
        line8.Subgroup.ShouldBe("TERMINATED");
        line8.NumberOfMembers.ShouldBe(1_784);
        line8.TotalWages.ShouldBe(4_669_803.88m);
        line8.TotalBalance.ShouldBe(69_592_085.49m);
        line8.TotalHours.ShouldBeNull(); // Hours/points only for Line 2
        line8.TotalPoints.ShouldBeNull();

        // X - < AGE 18 NO WAGES
        var lineX = lineItemsByPrefix["X"];
        lineX.LineItemPrefix.ShouldBe("X");
        lineX.LineItemTitle.ShouldContain("< AGE 18"); // Title includes "NO WAGES : 0"
        lineX.Subgroup.ShouldBe(string.Empty); // X is not in a subgroup
        lineX.NumberOfMembers.ShouldBe(2);
        lineX.TotalWages.ShouldBe(11_052.15m);
        lineX.TotalBalance.ShouldBe(0.00m);
        lineX.TotalHours.ShouldBeNull(); // Hours/points only for Line 2
        lineX.TotalPoints.ShouldBeNull();

        // N - NON-EMPLOYEE BENEFICIARIES
        var lineN = lineItemsByPrefix["N"];
        lineN.LineItemPrefix.ShouldBe("N");
        lineN.LineItemTitle.ShouldBe("NON-EMPLOYEE BENEFICIARIES");
        lineN.Subgroup.ShouldBe("TERMINATED"); // N appears under the TERMINATED header in R8-PAY426N-9
        lineN.NumberOfMembers.ShouldBe(169);
        lineN.TotalWages.ShouldBe(0.00m);
        lineN.TotalBalance.ShouldBe(2_564_290.61m);
        lineN.TotalHours.ShouldBeNull(); // Hours/points only for Line 2
        lineN.TotalPoints.ShouldBeNull();

        TestOutputHelper.WriteLine("✓ All 11 line items parsed correctly with matching values");
    }

    [Fact]
    [Description("Pay426N9Parser should handle empty report text")]
    public void Parse_WithEmptyText_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string emptyReport = string.Empty;

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => Pay426N9Parser.Parse(emptyReport))
            .Message.ShouldBe("Failed to parse any line items from PAY426N-9 summary report");
    }

    [Fact]
    [Description("Pay426N9Parser should handle report with only headers")]
    public void Parse_WithOnlyHeaders_ShouldThrowInvalidOperationException()
    {
        // Arrange
        string reportWithOnlyHeaders = @"PAY426N-S                            DEMOULAS SUPERMARKETS INC      PROFIT SHARING FOR   2025
- PROFIT SHARING SUMMARY TOTAL PAGE

ACTIVE AND INACTIVE:

TERMINATED:";

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => Pay426N9Parser.Parse(reportWithOnlyHeaders))
            .Message.ShouldBe("Failed to parse any line items from PAY426N-9 summary report");
    }
}
