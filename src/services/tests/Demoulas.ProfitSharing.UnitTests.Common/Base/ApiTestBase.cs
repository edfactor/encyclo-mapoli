using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Common.Base;

/// <summary>
///   Abstraction for testing api endpoints that use a <c>DbContext</c>.
///   NOTE: Each test CLASS (not method) gets its own factory instance.
///   Tests within the same [Collection] share the factory, so they may pollute each other's data.
///   Implements IAsyncDisposable to clean up ServiceProvider and HTTP clients after test completion.
/// </summary>
[Collection("SharedGlobalState")]
public class ApiTestBase<TStartup> : IAsyncDisposable where TStartup : class
{
    /// <summary>
    ///   Mock for DbContext.
    ///   Each test class gets a fresh instance, but tests within a class share this factory.
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

    /// <summary>
    ///   The WebApplicationFactory instance that manages the test server.
    ///   Must be disposed to release server resources (ports, threads, etc.) after tests complete.
    /// </summary>
    private readonly WebApplicationFactory<TStartup> _factory;

    public ServiceProvider? ServiceProvider { get; private set; }

    /// <summary>
    ///   Initializes a default instance of <c>ApiTestBase</c>
    /// </summary>
    /// <remarks>
    ///   Intentionally set as <c>protected</c> to prevent instances being created.
    /// </remarks>
    public ApiTestBase()
    {
        // Each test class gets its own FRESH factory with 6,500+ fake records
        MockDbContextFactory = MockDataContextFactory.InitializeForTesting();

        // Note: ASPNETCORE_ENVIRONMENT is set via TestModuleInitializer to ensure
        // it's configured before any WebApplicationFactory or ASP.NET host is created.
        // This is required for xUnit v3 / Microsoft Testing Platform compatibility.

        var webApplicationFactory = new WebApplicationFactory<TStartup>();

        _factory = webApplicationFactory.WithWebHostBuilder(
            hostBuilder =>
            {
                hostBuilder.UseEnvironment("Testing");

                hostBuilder.ConfigureServices(services =>
                {
                    services.AddTransient((_) => MockDbContextFactory);

                    services.AddTransient((_) => MockEmbeddedSqlService.Initialize());
                    services.AddTransient((_) => MockCalendarService.Initialize());

                    // Allow tests to provide additional service registrations before the provider is built.
                    TestServiceOverrides.Hook?.Invoke(services);

                    ServiceProvider = services.BuildServiceProvider();
                });
            });

        ApiClient = _factory.CreateClient();

        // Set timeout for integration tests - should complete quickly with mocked database
        // If this timeout is hit, it indicates a performance problem that needs investigation
        // Derived classes can override GetHttpClientTimeout() to customize this value
        ApiClient.Timeout = GetHttpClientTimeout() ?? TimeSpan.FromMinutes(2);

        DownloadClient = _factory.CreateClient();
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

    /// <summary>
    /// Disposes test resources to reduce memory accumulation during test suite execution.
    /// Called by xUnit test runner after each test class completes.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        // Dispose ServiceProvider asynchronously to release all registered services and their resources
        if (ServiceProvider != null)
        {
            await ServiceProvider.DisposeAsync();
        }

        // Dispose HTTP clients to close connections and free resources
        ApiClient?.Dispose();
        DownloadClient?.Dispose();

        // Dispose WebApplicationFactory to release test server resources (ports, threads, etc.)
        // This is critical to prevent resource exhaustion when running many test classes
        await _factory.DisposeAsync();
    }

}
