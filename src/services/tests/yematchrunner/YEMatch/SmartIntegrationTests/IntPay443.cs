namespace YEMatch.SmartIntegrationTests;

/// <summary>
///     Runs the SMART integration test for PAY443 report processing.
/// </summary>
public class IntPay443 : BaseIntegrationTestActivity
{
    public IntPay443(string integrationTestPath)
        : base(integrationTestPath, "PAY443", "Pay443Test")
    {
    }
}
