namespace YEMatch;

public abstract class Activity
{
    public string prefix { get; set; } = null!;

    public abstract string ActivityLetterNumber { get; set; }
    public abstract Task<Outcome> execute();
}
