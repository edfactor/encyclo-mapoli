namespace YEMatch.YEMatch.Activities;

// Combines two activities into one.   The activities are run in parallel.  Typically this is so READY and SMART can run activties at the saem time.
public sealed class ParallelActivity : IActivity
{
    private readonly IActivity _first;
    private readonly string _name;
    private readonly IActivity _second;

    public ParallelActivity(string name, IActivity first, IActivity second)
    {
        _name = name;
        _first = first;
        _second = second;
    }

    public string Name()
    {
        return _name;
    }

    public async Task<Outcome> Execute()
    {
        Task<Outcome> firstTask = _first.Execute();
        Task<Outcome> secondTask = _second.Execute();

        await Task.WhenAll(firstTask, secondTask);

        Outcome firstOutcome = await firstTask;
        Outcome secondOutcome = await secondTask;

        return firstOutcome.Merge(secondOutcome);
    }
}
