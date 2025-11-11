using YEMatch.Activities;

namespace YEMatch.SmartActivities;

/// <summary>
///     Factory interface for creating SMART API clients
/// </summary>
public interface ISmartApiClientFactory
{
    /// <summary>
    ///     Creates a configured API client for SMART
    /// </summary>
    ApiClient CreateClient();

    /// <summary>
    ///     Creates SMART activities
    /// </summary>
    List<IActivity> CreateActivities(string dataDirectory);
}
