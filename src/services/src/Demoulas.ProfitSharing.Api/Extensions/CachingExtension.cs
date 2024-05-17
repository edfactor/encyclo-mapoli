

namespace Demoulas.ProfitSharing.Api.Extensions;

/// <summary>
/// Provides helper methods for configuring project services.
/// </summary>
public static class CachingExtension
{
    public static WebApplicationBuilder AddCachingServices(this WebApplicationBuilder builder)
    {
        _ = builder.Services.AddDistributedMemoryCache();

        if (builder.Environment.EnvironmentName != "Testing")
        {
            //_ = builder.Services.AddHostedService<AccountCacheHostedService>();
        }

        return builder;
    }
}
