namespace YEMatch.SmartIntegrationTests;

/// <summary>
///     Runs the SMART integration test for PAY426N9 report processing.
/// </summary>
public class IntPay426N9 : BaseIntegrationTestActivity
{
    public IntPay426N9(string integrationTestPath)
        : base(integrationTestPath, "PAY426N9", "Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY426N.Pay426N9Tests.Pay426N9Summary_ShouldMatchReady")
    {
    }
}
