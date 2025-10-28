using YEMatch.YEMatch.Activities;

namespace YEMatch.YEMatch.SmartIntegrationTests;

public static class IntegrationTestFactory
{
    public static List<IActivity> CreateActivities(string dataDirectory)
    {
        return
        [
            // Test
            new IntPay443(),
            new IntTerminatedEmployee()
        ];
    }
}
