using System.Diagnostics;
using YEMatch.YEMatch.Activities;

namespace YEMatch.YEMatch.SmartActivities;

#pragma warning disable CS1998

public class SmartActivity : IActivity
{
    private readonly ApiClient _client;
    private readonly Func<ApiClient, string, string, Task<Outcome>> _func;
    private readonly string Command;
    private readonly string name;

    public SmartActivity(Func<ApiClient, string, string, Task<Outcome>> func, ApiClient client, string ActivityLetterNumber, string command)
    {
        _func = func;
        _client = client;
        name = ActivityLetterNumber.Substring(0, 1).Replace("A", "S") + ActivityLetterNumber.Substring(1);
        Command = command;
    }


    public string Name()
    {
        return name;
    }

    public async Task<Outcome> Execute()
    {
        Console.WriteLine($"SMART>         {name} {Command}  - start at: {DateTime.Now}");
        Stopwatch stopwatch = new();
        stopwatch.Start();
        Outcome outcome = await _func(_client, name, Command);
        stopwatch.Stop();
        if (outcome.took == null && outcome.Status == OutcomeStatus.Ok)
        {
            return outcome with { took = stopwatch.Elapsed };
        }

        return outcome;
    }
}
