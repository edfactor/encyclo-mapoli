using YEMatch.YEMatch.Activities;

namespace YEMatch.YEMatch.AssertActivities;

// The base class for activities which directly interact with the database
public abstract class BaseActivity : IActivity
{
    public virtual string Name()
    {
        return GetType().Name;
    }

    public abstract Task<Outcome> Execute();
}
