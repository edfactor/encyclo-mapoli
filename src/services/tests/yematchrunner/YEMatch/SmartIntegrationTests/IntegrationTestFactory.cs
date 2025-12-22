using Microsoft.Extensions.Options;
using YEMatch.Activities;

namespace YEMatch.SmartIntegrationTests;

/// <summary>
///     Factory for creating integration test activities with proper dependency injection.
/// </summary>
public sealed class IntegrationTestFactory : IIntegrationTestFactory
{
    private readonly string _integrationTestPath;

    public IntegrationTestFactory(IOptions<YeMatchOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _integrationTestPath = options.Value.Paths.GetIntegrationTestResourcesPath();
    }

    public List<IActivity> CreateActivities()
    {
        return
        [
            new IntPay443(_integrationTestPath),
            new IntTerminatedEmployee(_integrationTestPath),
            new IntTestPay426DataUpdates(_integrationTestPath),
            new IntTestQPay129(_integrationTestPath),
            new IntPay426N9(_integrationTestPath),
            new IntPay426N(_integrationTestPath),
            new IntPay426(_integrationTestPath),
            new IntProfitMasterUpdateTest(_integrationTestPath),
            new IntPay450(_integrationTestPath),
            new IntPay444Test(_integrationTestPath),
            new IntPay447Test(_integrationTestPath)
        ];
    }
}
