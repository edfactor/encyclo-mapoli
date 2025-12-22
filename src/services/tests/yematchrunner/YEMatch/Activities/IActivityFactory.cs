namespace YEMatch.Activities;

/// <summary>
///     Factory interface for creating and managing activities
/// </summary>
public interface IActivityFactory
{
    /// <summary>
    ///     Gets all activities indexed by their enum name
    /// </summary>
    Dictionary<ActivityName, IActivity> GetActivitiesByName();

    /// <summary>
    ///     Gets a specific activity by enum name
    /// </summary>
    IActivity GetActivity(ActivityName name);

    /// <summary>
    ///     Checks if the new scramble is being used
    /// </summary>
    bool IsNewScramble();

    /// <summary>
    ///     Sets the new scramble mode
    /// </summary>
    void SetNewScramble();
}
