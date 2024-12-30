using System.Net.Http.Headers;
using System.Text;
using Demoulas.ProfitSharing.OracleHcm.Atom;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.HostedServices;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.OracleHcm.Extensions;

public static class OracleHcmExtension
{
    private const string FrameworkVersionHeader = "REST-Framework-Version";

    public static IHostApplicationBuilder AddOracleHcmBackgroundProcess(this IHostApplicationBuilder builder)
    {
        _ = builder.AddOracleHcmSynchronization();
        _ = builder.Services.AddHostedService<AtomFeedHostedService>();

        return builder;
    }

    public static IHostApplicationBuilder AddOracleHcmSynchronization(this IHostApplicationBuilder builder)
    {
        OracleHcmConfig oracleHcmConfig = builder.Configuration.GetSection("OracleHcm").Get<OracleHcmConfig>() ??
                                          new OracleHcmConfig { BaseAddress = string.Empty, DemographicUrl = string.Empty };

        _ = builder.Services.AddSingleton(oracleHcmConfig);

        // Add Atom feed-based services
        _ = builder.Services.AddSingleton<AtomFeedService>();
        _ = builder.Services.AddSingleton<DemographicsService>();
        _ = builder.Services.AddSingleton<EmployeeMapper>();
        _ = builder.Services.AddSingleton<SyncJobManager>();

        _ = builder.Services.AddSingleton<DemographicMapper>();
        _ = builder.Services.AddSingleton<AddressMapper>();
        _ = builder.Services.AddSingleton<ContactInfoMapper>();


        // Add HttpClient with authorization for Atom feeds
        _ = builder.Services.AddHttpClient<AtomFeedService>(BuildOracleHcmAuthClient);

        return builder;
    }

    private static void BuildOracleHcmAuthClient(IServiceProvider services, HttpClient client)
    {
        OracleHcmConfig config = services.GetRequiredService<OracleHcmConfig>();

        byte[] bytes = Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}");
        string encodedAuth = Convert.ToBase64String(bytes);
        client.BaseAddress = new Uri(config.BaseAddress, UriKind.Absolute);
        client.DefaultRequestHeaders.Add(FrameworkVersionHeader, config.RestFrameworkVersion);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuth);

        // Specify JSON format
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
