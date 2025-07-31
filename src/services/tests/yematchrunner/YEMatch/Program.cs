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

        // Runnable runner = new BaselineRun() { DataDirectory = dataDirectory };
        // Runnable runner = new GoldenRun { DataDirectory = dataDirectory };

        // Runnable runner = new MasterInquiryRun { DataDirectory = dataDirectory };

        // Runnable runner = new GoldenExpressRun { DataDirectory = dataDirectory };
        Runnable runner = new TinkerRun() { DataDirectory = dataDirectory };
        await runner.Exec();
    }
}
