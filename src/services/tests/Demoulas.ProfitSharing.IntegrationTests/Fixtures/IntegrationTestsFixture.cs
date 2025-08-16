using FastEndpoints.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.IntegrationTests.Fixtures;

public class IntegrationTestsFixture : AppFixture<Program>
{
    public IntegrationTestsFixture()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
    }

    protected override void ConfigureServices(IServiceCollection s)
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables(); // You can add other configuration sources here
        var configuration = configurationBuilder.Build();
        s.AddSingleton<IConfiguration>(configuration);

        base.ConfigureServices(s);
    }
}
