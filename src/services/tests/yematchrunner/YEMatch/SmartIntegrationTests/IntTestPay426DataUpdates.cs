namespace YEMatch.SmartIntegrationTests;

/// <summary>
///     Runs the SMART integration test for PAY426 data updates validation.
/// </summary>
public class IntTestPay426DataUpdates : BaseIntegrationTestActivity
{
    public IntTestPay426DataUpdates(string integrationTestPath)
        : base(integrationTestPath, "TestPay426DataUpdates", "Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.YearEndServiceTests.TestPay426DataUpdates")
    {
    }
}
