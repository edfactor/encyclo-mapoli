using System.Net.Http.Headers;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;

/**
 * WARNING - needs a refactor.   It is locked to the ExecutiveHoursAndDollarsResponse type at the moment.
 *
 * This class should be refactored to have a proper database connection to the pristine obfuscated database.
 *
 * now it uses a mock to get the testing framework to swap in the read only connection.
 *
 */
/// <summary>
///   Abstraction for testing api endpoints that use a <c>DbContext</c>.
///   sets up a web application, endpoints and services and a real read only database connection.
/// </summary>
public class ApiIntegrationTestBase<TStartup> where TStartup : class
{
    /// <summary>
    ///    Not quite mock, this factory has a ready only connection to the read only version of the SMART obfuscated pristine database.
    /// </summary>
    public IProfitSharingDataContextFactory SomeWhatMockDbContextFactory { get; }

    /// <summary>
    ///   The client to invoke api endpoints.
    /// </summary>
    public HttpClient ApiClient { get; }

    public HttpClient DownloadClient { get; }

    public ServiceProvider? ServiceProvider { get; private set; }

    /// <summary>
    ///   Initializes a default instance of <c>ApiIntegrationTestBase</c>
    /// </summary>
    /// <remarks>
    ///   Intentionally set as <c>protected</c> to prevent instances being created.
    /// </remarks>
    protected ApiIntegrationTestBase()
    {
        // We get a connection to the SMART obfuscated pristine database.
        var configuration = new ConfigurationBuilder().AddUserSecrets<TStartup>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing"]!;
        var options = new DbContextOptionsBuilder<ProfitSharingReadOnlyDbContext>().UseOracle(connectionString).EnableSensitiveDataLogging().Options;
        var ctx = new ProfitSharingReadOnlyDbContext(options);

        // Probably should simply use a normal database connection (aka proper instance of IProfitSharingDataContextFactory)
        // but for now, use a mock.
        var profitSharingDataContextFactoryMock = new Mock<IProfitSharingDataContextFactory>();

        // WARNING: note the very specific type matching required for this mock to get invoked, not general enough for other integration tests
        profitSharingDataContextFactoryMock
            .Setup(factory => factory.UseReadOnlyContext(It.IsAny<Func<ProfitSharingReadOnlyDbContext, Task<PaginatedResponseDto<ExecutiveHoursAndDollarsResponse>>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<ProfitSharingReadOnlyDbContext, Task<PaginatedResponseDto<ExecutiveHoursAndDollarsResponse>>>, CancellationToken>((func, ct) => func(ctx));

        SomeWhatMockDbContextFactory = profitSharingDataContextFactoryMock.Object;

        WebApplicationFactory<TStartup> webApplicationFactory = new WebApplicationFactory<TStartup>();

        WebApplicationFactory<TStartup> builder = webApplicationFactory.WithWebHostBuilder(
            hostBuilder =>
            {
                hostBuilder.UseEnvironment("Testing");
                hostBuilder.ConfigureServices(services =>
                {
                    services.AddTransient((c) => SomeWhatMockDbContextFactory);
                    ServiceProvider = services.BuildServiceProvider();
                });
            });

        ApiClient = builder.CreateClient();
        DownloadClient = builder.CreateClient();
        DownloadClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/csv"));
    }
}
