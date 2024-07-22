
using Demoulas.ProfitSharing.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.OracleHcm.Cli;

public class Program
{
    public static async Task Main()
    {
        var services = new ServiceCollection();
        services.AddHttpClient<DemographicsService>().AddStandardResilienceHandler();

        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddUserSecrets<Program>();
        var config = configurationBuilder.Build();

        // Load Okta settings
        OracleHcmConfig oktaSettings = config.GetSection("OracleHcm").Get<OracleHcmConfig>() ?? new OracleHcmConfig();

        var provider = services.BuildServiceProvider();

        var service = provider.GetRequiredService<DemographicsService>();

        var result = await service.GetAllEmployees(oktaSettings);

        Console.WriteLine(result);
        Console.ReadLine();
    }
}
