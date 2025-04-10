using System.Diagnostics;

namespace YEMatch;

#pragma warning disable CS1998

public class SmartActivity : Activity
{
    public static readonly string smartPrefix = "                                                   ";
    private readonly ApiClient _client;
    private readonly Func<ApiClient, string, string, Task<Outcome>> _func;
    private readonly string Command;

    public SmartActivity(Func<ApiClient, string, string, Task<Outcome>> func, ApiClient client, string ActivityLetterNumber, string command)
    {
        _func = func;
        _client = client;
        this.ActivityLetterNumber = ActivityLetterNumber;
        Command = command;
        prefix = smartPrefix;
    }

    public override string ActivityLetterNumber { get; set; }

    public override async Task<Outcome> execute()
    {
        Console.WriteLine($"{prefix}{ActivityLetterNumber} {Command}  - start at: {DateTime.Now}");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var outcome = await _func(_client, ActivityLetterNumber, Command);
        stopwatch.Stop();
        if (outcome.took == null && outcome.Status == OutcomeStatus.Ok)
        {
            return outcome with { took = stopwatch.Elapsed };
        }

        return outcome;
    }
}
