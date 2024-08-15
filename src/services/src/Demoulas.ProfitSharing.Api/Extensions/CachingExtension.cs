

using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Services.HostedServices;

namespace Demoulas.ProfitSharing.Api.Extensions;

/// <summary>
/// Provides helper methods for configuring project services.
/// </summary>
public static class CachingExtension
{
    public static WebApplicationBuilder AddCachingServices(this WebApplicationBuilder builder)
    {
        _ = builder.Services.AddDistributedMemoryCache();

        if (!builder.Environment.IsTestEnvironment())
        {
            _ = builder.Services.AddHostedService<PayClassificationHostedService>();
            //_ = builder.Services.AddHostedService<StoreHostedService>(); // TODO: Does PS care about specific stores???
        }

        return builder;
    }
}
