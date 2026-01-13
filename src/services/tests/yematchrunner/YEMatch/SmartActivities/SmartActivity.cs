using System.Diagnostics;
using System.Text.RegularExpressions;
using YEMatch.Activities;

namespace YEMatch.SmartActivities;

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
        name = FormatActivityName(ActivityLetterNumber);
        Command = command;
    }

    // Parse ActivityLetterNumber like "A1", "A13A", "A24B" and return zero-padded "S01", "S13A", "S24B"
    private static string FormatActivityName(string activityLetterNumber)
    {
        Match match = Regex.Match(activityLetterNumber, @"^A(\d+)(\D*)$");
        if (match.Success)
        {
            string number = match.Groups[1].Value.PadLeft(2, '0');
            string suffix = match.Groups[2].Value;
            return $"S{number}{suffix}";
        }

        // Fallback to original behavior
        return activityLetterNumber.Substring(0, 1).Replace("A", "S") + activityLetterNumber.Substring(1);
    }


    public string Name()
    {
        return name;
    }

    public async Task<Outcome> Execute()
    {
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
