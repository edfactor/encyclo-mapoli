namespace YEMatch;


public sealed class ParallelActivity : IActivity
{
    private readonly string _name;
    private readonly IActivity _first;
    private readonly IActivity _second;

    public ParallelActivity(string name, IActivity first, IActivity second)
    {
        _name = name;
        _first = first;
        _second = second;
    }

    public string Name() => _name;

    public async Task<Outcome> Execute()
    {
        var firstTask = _first.Execute();
        var secondTask = _second.Execute();

        await Task.WhenAll(firstTask, secondTask);

        var firstOutcome = await firstTask;
        var secondOutcome = await secondTask;

        return firstOutcome.Merge(secondOutcome);
    }
}
