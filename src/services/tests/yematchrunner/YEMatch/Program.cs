using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;


#pragma warning disable CS0162 // Unreachable code detected
// ReSharper disable FieldCanBeMadeReadOnly.Local
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable S1144
#pragma warning disable AsyncFixer01

namespace YEMatch;

[SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
[SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed")]
[SuppressMessage("Style", "IDE0044:Add readonly modifier")]
internal static class Program
{
    private static async Task Main(string[] args)
    {
        string dataDirectory = Config.CreateDataDirectory();
        ActivityFactory.Initialize(dataDirectory);

        Runnable runner = new GoldenExpress { DataDirectory = dataDirectory };
        await runner.Exec();
    }
}
