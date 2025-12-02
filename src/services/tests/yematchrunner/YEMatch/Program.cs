using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.Runs;
using YEMatch.SmartActivities;
using YEMatch.SmartIntegrationTests;

#pragma warning disable CS0162 // Unreachable code detected
// ReSharper disable FieldCanBeMadeReadOnly.Local
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable S1144
#pragma warning disable AsyncFixer01

namespace YEMatch;

/// <summary>
///     Marker class for user secrets (Program is static, can't be used as type parameter)
/// </summary>
[SuppressMessage("Major Code Smell", "S2094:Classes should not be empty")]
internal sealed class ProgramMarker
{
}

[SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
[SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed")]
[SuppressMessage("Style", "IDE0044:Add readonly modifier")]
[SuppressMessage("Major Code Smell", "S1135:Track uses of 'TODO' tags")]
internal static class Program
{
    private static async Task Main(string[] args)
    {
        // Create data directory early (before DI setup)
        string dataDirectory = Config.CreateDataDirectory();

        // Configure Serilog - verbose to file, minimal to console
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                restrictedToMinimumLevel: LogEventLevel.Warning,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                Path.Combine(dataDirectory, "yematch-.log"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        try
        {
            Log.Information("YEMatch starting with data directory: {DataDirectory}", dataDirectory);

            // Build host with DI
            using IHost host = Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", true, false);
                    config.AddUserSecrets<ProgramMarker>(true);
                })
                .ConfigureServices((context, services) =>
                {
                    // Configuration
                    services.Configure<YeMatchOptions>(context.Configuration.GetSection(YeMatchOptions.SectionName));

                    // Update YeMatchOptions with runtime data directory
                    services.PostConfigure<YeMatchOptions>(options => { options.BaseDataDirectory = dataDirectory; });

                    // HttpClient for SMART API
                    services.AddHttpClient("SmartApi", (sp, client) =>
                    {
                        YeMatchOptions options = sp.GetRequiredService<IOptions<YeMatchOptions>>().Value;
                        client.BaseAddress = new Uri(options.SmartApi.BaseUrl);
                        client.Timeout = TimeSpan.FromHours(options.SmartApi.TimeoutHours);
                    });

                    // Factories (singleton for lifetime management of SSH/HTTP clients)
                    services.AddSingleton<IReadySshClientFactory, ReadySshClientFactory>();
                    services.AddSingleton<ISmartApiClientFactory, SmartApiClientFactory>();
                    services.AddSingleton<IIntegrationTestFactory, IntegrationTestFactory>();
                    services.AddSingleton<IActivityFactory>(sp =>
                    {
                        IReadySshClientFactory readyFactory = sp.GetRequiredService<IReadySshClientFactory>();
                        ISmartApiClientFactory smartFactory = sp.GetRequiredService<ISmartApiClientFactory>();
                        IIntegrationTestFactory integrationFactory = sp.GetRequiredService<IIntegrationTestFactory>();
                        return new ActivityFactory(readyFactory, smartFactory, integrationFactory, dataDirectory);
                    });

                    // Register all Run types as transient (new instance per request)
                    services.AddTransient<BaselineRun>();
                    services.AddTransient<GoldenYearEndRun>();
                    services.AddTransient<GoldenExpressRun>();
                    services.AddTransient<GoldenDecemberRun>();
                    services.AddTransient<MasterInquiryRun>();
                    services.AddTransient<TinkerRun>();
                    services.AddTransient<TerminationsRun>();
                    services.AddTransient<SevenRun>();
                    services.AddTransient<ViewRun>();
                    services.AddTransient<AutoMatchRun>();
                    services.AddTransient<FrozenTestingRun>();
                    services.AddTransient<Only14EmployeesRun>();
                })
                .Build();

            // Parse run type
            string? runType = ParseRunType(args);
            Log.Information("Run type: {RunType}", runType ?? "not specified");

            // Get runner from DI container
            Runnable runner = GetRunner(host.Services, runType, dataDirectory);
            await runner.Exec();

            Log.Information("YEMatch completed successfully");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "YEMatch terminated unexpectedly");
            throw;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static string? ParseRunType(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--run" && i + 1 < args.Length)
            {
                return args[i + 1].ToLowerInvariant();
            }
        }

        return null;
    }

    private static Runnable GetRunner(IServiceProvider services, string? runType, string dataDirectory)
    {
        Runnable runner = runType switch
        {
            "baseline" => services.GetRequiredService<BaselineRun>(),
            "goldenyearend" => services.GetRequiredService<GoldenYearEndRun>(),
            "goldenexpress" => services.GetRequiredService<GoldenExpressRun>(),
            "goldendecemberexpress" => services.GetRequiredService<GoldenDecemberRun>(),
            "masterinquiry" => services.GetRequiredService<MasterInquiryRun>(),
            "tinker" => services.GetRequiredService<TinkerRun>(),
            "terminations" => services.GetRequiredService<TerminationsRun>(),
            "seven" => services.GetRequiredService<SevenRun>(),
            "view" => services.GetRequiredService<ViewRun>(),
            "automatch" => services.GetRequiredService<AutoMatchRun>(),
            _ => throw new ArgumentException($"Unknown run type: {runType}")
        };

        runner.DataDirectory = dataDirectory;
        return runner;
    }
}
