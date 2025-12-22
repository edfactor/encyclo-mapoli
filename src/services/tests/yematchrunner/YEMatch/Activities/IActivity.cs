namespace YEMatch.Activities;

/*
 * This is a generic task in yerunner, not just a ye activity.
 */
public interface IActivity
{
    string Name();
    Task<Outcome> Execute();

}
