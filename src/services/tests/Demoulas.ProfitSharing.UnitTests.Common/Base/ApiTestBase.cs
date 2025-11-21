using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests.Common.Base;

/// <summary>
/// Static holder for the cached mock DbContext factory to avoid Sonar S2743 warning
/// (static fields in generic types are not shared among different close constructed types).
/// This ensures factory caching works across all test classes regardless of their TStartup type.
/// </summary>
internal static class MockDbContextFactoryCache
{
    private static IProfitSharingDataContextFactory? s_cachedFactory;
    private static readonly object s_lockObject = new object();

    public static IProfitSharingDataContextFactory GetOrCreate()
    {
        if (s_cachedFactory == null)
        {
            lock (s_lockObject)
            {
                s_cachedFactory ??= MockDataContextFactory.InitializeForTesting();
            }
        }
        return s_cachedFactory;
    }
}

/// <summary>
///   Abstraction for testing api endpoints that use a <c>DbContext</c>.
/// </summary>
public class ApiTestBase<TStartup> where TStartup : class
{
    /// <summary>
    ///   Mock for DbContext.
    /// </summary>
    public IProfitSharingDataContextFactory MockDbContextFactory { get; set; }



    /// <summary>
    ///   The client to invoke api endpoints.
    /// </summary>
    public HttpClient ApiClient { get; }

    public HttpClient DownloadClient { get; }

    /// <summary>
    ///   Virtual method to allow derived classes to specify a custom HTTP client timeout.
    ///   Override this method to return a custom timeout value.
    /// </summary>
    /// <returns>The HTTP client timeout, or null to use the default 2-minute timeout.</returns>
    protected virtual TimeSpan? GetHttpClientTimeout() => null;


    public ServiceProvider? ServiceProvider { get; private set; }

    /// <summary>
    ///   Initializes a default instance of <c>ApiTestBase</c>
    /// </summary>
    /// <remarks>
    ///   Intentionally set as <c>protected</c> to prevent instances being created.
    /// </remarks>
    public ApiTestBase()
    {
        // Use cached factory to avoid generating 6,500 fake records for every test.
        // Factory creation is expensive (~60-80 seconds), but after first test it's reused.
        // Expected: 20+ minute test run → 10 minute test run (50% time savings).
        MockDbContextFactory = MockDbContextFactoryCache.GetOrCreate();

        WebApplicationFactory<TStartup> webApplicationFactory = new WebApplicationFactory<TStartup>();

        WebApplicationFactory<TStartup> builder = webApplicationFactory.WithWebHostBuilder(
            hostBuilder =>
            {
                hostBuilder.UseEnvironment("Testing");

                hostBuilder.ConfigureServices(services =>
                {
                    services.AddTransient((_) => MockDbContextFactory);

                    services.AddTransient((_) => MockEmbeddedSqlService.Initialize());
                    services.AddTransient<ICalendarService, CalendarService>();

                    // Allow tests to provide additional service registrations before the provider is built.
                    TestServiceOverrides.Hook?.Invoke(services);

                    ServiceProvider = services.BuildServiceProvider();
                });
            });

        ApiClient = builder.CreateClient();

        // Set timeout for integration tests - should complete quickly with mocked database
        // If this timeout is hit, it indicates a performance problem that needs investigation
        // Derived classes can override GetHttpClientTimeout() to customize this value
        ApiClient.Timeout = GetHttpClientTimeout() ?? TimeSpan.FromMinutes(2);

        DownloadClient = builder.CreateClient();
        DownloadClient.Timeout = GetHttpClientTimeout() ?? TimeSpan.FromMinutes(2);
        DownloadClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/csv"));
    }

    /// <summary>
    /// Retrieves the year with the maximum profit from the database.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the year with the maximum profit as a <see cref="short"/>.</returns>
    public Task<short> GetMaxProfitYearAsync(CancellationToken cancellationToken = default)
    {
        return MockDbContextFactory.UseReadOnlyContext(async ctx =>
        {
            return await ctx.PayProfits.MaxAsync(pp => pp.ProfitYear, cancellationToken: cancellationToken);
        }, cancellationToken);
    }

}
