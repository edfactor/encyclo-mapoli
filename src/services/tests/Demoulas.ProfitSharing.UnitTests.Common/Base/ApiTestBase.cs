using System.Diagnostics;
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


    public ServiceProvider? ServiceProvider { get; private set; }
    
    /// <summary>
    ///   Initializes a default instance of <c>ApiTestBase</c>
    /// </summary>
    /// <remarks>
    ///   Intentionally set as <c>protected</c> to prevent instances being created.
    /// </remarks>
    public ApiTestBase()
    {
        MockDbContextFactory = MockDataContextFactory.InitializeForTesting();
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

        // When debugging, the 100-second default goes by quickly.
        if (Debugger.IsAttached)
        {
            ApiClient.Timeout = TimeSpan.FromMinutes(30);
        }

        DownloadClient = builder.CreateClient();
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
        });
    }

}
