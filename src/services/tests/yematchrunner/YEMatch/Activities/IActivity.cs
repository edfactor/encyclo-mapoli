namespace YEMatch.YEMatch.Activities;

/*
 * This is a generic task in yerunner, not just a ye activity.
 */
public interface IActivity
{
    public string Name();
    public Task<Outcome> Execute();
}
