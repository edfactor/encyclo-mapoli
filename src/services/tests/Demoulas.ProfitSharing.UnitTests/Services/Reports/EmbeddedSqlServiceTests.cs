using System.ComponentModel;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Services.Services.Reports;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services.Reports;

[Collection("SharedGlobalState")]
public sealed class EmbeddedSqlServiceTests
{
    [Fact]
    [Description("PS-2524 : Vesting ratio query uses FullVestingYears for 65/5 rule")]
    public void GetVestingRatioQuery_UsesFullVestingYearsForInitialContributionThreshold()
    {
        var profitYear = (short)2025;
        var asOfDate = new DateOnly(2025, 12, 27);

        var query = EmbeddedSqlService.GetVestingRatioQuery(profitYear, asOfDate);

        var expectedYear = asOfDate.AddYears(-ReferenceData.FullVestingYears).Year;
        query.ShouldContain($"m.initial_contr_year <= {expectedYear}");
    }

    [Fact]
    [Description("PS-2524 : Vesting ratio 65/5 applies only to active employees")]
    public void GetVestingRatioQuery_Restricts65PlusVestingToActiveEmployees()
    {
        var profitYear = (short)2025;
        var asOfDate = new DateOnly(2025, 12, 27);

        var query = EmbeddedSqlService.GetVestingRatioQuery(profitYear, asOfDate);
        var normalized = query.Replace(" ", string.Empty)
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\t", string.Empty);

        normalized.ShouldContain("CASEWHENm.ZERO_CONTRIBUTION_REASON_ID=6ANDm.IS_EMPLOYEE=1AND(m.termination_dateISNULLORm.termination_date>TO_DATE('");
        normalized.ShouldContain("m.IS_EMPLOYEE=1AND(m.termination_dateISNULLORm.termination_date>TO_DATE('");
    }
}
