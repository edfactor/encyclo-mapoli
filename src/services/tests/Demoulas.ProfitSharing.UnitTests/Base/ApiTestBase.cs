using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests.Base;

/// <summary>
///   Abstraction for testing api endpoints that use a <c>DbContext</c>.
/// </summary>
public class ApiTestBase<TStartup> where TStartup : class
{

    /// <summary>
    ///   Mock for DbContext.
    /// </summary>
    public IProfitSharingDataContextFactory MockDbContextFactory { get; }



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
                    services.AddSingleton(MockDbContextFactory);

                    ServiceProvider = services.BuildServiceProvider();
                });
            });

        ApiClient = builder.CreateClient();
        DownloadClient = builder.CreateClient();

        DownloadClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/csv"));
    }
}
