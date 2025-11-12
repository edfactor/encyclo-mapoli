namespace YEMatch.SmartIntegrationTests;

/// <summary>
///     Runs the SMART integration test for PAY426 report processing.
/// </summary>
public class IntPay426 : BaseIntegrationTestActivity
{
    public IntPay426(string integrationTestPath)
        : base(integrationTestPath, "PAY426", "Pay426Test")
    {
    }
}
