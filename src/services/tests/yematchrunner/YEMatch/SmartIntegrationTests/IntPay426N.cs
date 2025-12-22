namespace YEMatch.SmartIntegrationTests;

/// <summary>
///     Runs the SMART integration test for PAY426N report processing.
/// </summary>
public class IntPay426N : BaseIntegrationTestActivity
{
    public IntPay426N(string integrationTestPath)
        : base(integrationTestPath, "PAY426N", "Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY426N.Pay426NTests.Pay426NReport_ShouldMatchReady")
    {
    }
}
