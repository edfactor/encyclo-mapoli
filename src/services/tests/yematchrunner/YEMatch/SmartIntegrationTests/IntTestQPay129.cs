namespace YEMatch.SmartIntegrationTests;

/// <summary>
///     Runs the SMART integration test for QPAY129 quarterly processing.
/// </summary>
public class IntTestQPay129 : BaseIntegrationTestActivity
{
    public IntTestQPay129(string integrationTestPath)
        : base(integrationTestPath, "TestQPAY129", "Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.QPAY129.QPAY129Test.TestQPAY129")
    {
    }
}
