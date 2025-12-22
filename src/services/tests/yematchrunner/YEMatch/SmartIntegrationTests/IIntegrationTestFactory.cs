using YEMatch.Activities;

namespace YEMatch.SmartIntegrationTests;

/// <summary>
///     Factory for creating integration test activities that run SMART integration tests.
/// </summary>
public interface IIntegrationTestFactory
{
    /// <summary>
    ///     Creates all integration test activities.
    /// </summary>
    List<IActivity> CreateActivities();
}
