using System.Diagnostics.CodeAnalysis;
using YEMatch.YEMatch.Activities;
using YEMatch.YEMatch.Runs;

#pragma warning disable CS0162 // Unreachable code detected
// ReSharper disable FieldCanBeMadeReadOnly.Local
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable S1144
#pragma warning disable AsyncFixer01

namespace YEMatch.YEMatch;

[SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
[SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed")]
[SuppressMessage("Style", "IDE0044:Add readonly modifier")]
internal static class Program
{
    private static async Task Main(string[] args)
    {
        string dataDirectory = Config.CreateDataDirectory();
        ActivityFactory.Initialize(dataDirectory);

        Runnable runner = GetRunner(args, dataDirectory);
        await runner.Exec();
    }

    private static Runnable GetRunner(string[] args, string dataDirectory)
    {
        string? runType = null;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--run" && i + 1 < args.Length)
            {
                runType = args[i + 1].ToLowerInvariant();
                break;
            }
        }

        return runType switch
        {
            "baseline" => new BaselineRun { DataDirectory = dataDirectory },
            "goldenyearendrun" => new GoldenYearEndRun { DataDirectory = dataDirectory },
            "goldenexpress" => new GoldenExpressRun { DataDirectory = dataDirectory },
            "goldendecemberexpress" => new GoldenDecemberRun() { DataDirectory = dataDirectory },
            "masterinquiry" => new MasterInquiryRun { DataDirectory = dataDirectory },
            "tinker" => new TinkerRun { DataDirectory = dataDirectory },
            "seven" => new SevenRun { DataDirectory = dataDirectory },
            "view" => new ViewRun { DataDirectory = dataDirectory },
            _ => throw new ArgumentException($"Unknown run type: {runType}. Valid options are: baseline, golden, goldenexpress, masterinquiry, tinker, view")
        };
    }
}
