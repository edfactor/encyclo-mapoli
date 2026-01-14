namespace YEMatch.SmartIntegrationTests;

/// <summary>
///     Runs the SMART integration test for terminated employee and beneficiary report processing.
/// </summary>
public class IntTerminatedEmployee : BaseIntegrationTestActivity
{
    public IntTerminatedEmployee(string integrationTestPath)
        : base(integrationTestPath, "TerminatedEmployee", "Demoulas.ProfitSharing.IntegrationTests.Reports.Termination.TerminatedEmployeeAndBeneficiaryReportIntegrationTests.EnsureSmartReportMatchesReadyReport")
    {
    }
}
