
using System.Diagnostics;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Contracts.Request;
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
        OracleHcmConfig oktaSettings = config.GetSection("OracleHcm").Get<OracleHcmConfig>() ?? new OracleHcmConfig {Url = string.Empty};

        var provider = services.BuildServiceProvider();

        var service = provider.GetRequiredService<DemographicsService>();

        var employees = service.GetAllEmployees(oktaSettings);

        await foreach (OracleEmployee? employee in employees)
        {
            //var json = JsonSerializer.Serialize(employee, new JsonSerializerOptions { WriteIndented = Debugger.IsAttached });
            //Console.WriteLine(json);
        }

        
        Console.ReadLine();
    }

   
}
