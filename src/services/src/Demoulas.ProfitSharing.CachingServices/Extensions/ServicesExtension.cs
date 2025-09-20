using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Common.Caching;
using Demoulas.ProfitSharing.Services.Caching.HostedServices;
using Demoulas.Util.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Services.Caching.Extensions;

/// <summary>
/// Provides helper methods for configuring project builder.Services.
/// </summary>
public static class ServicesExtension
{
    public static IHostApplicationBuilder AddProjectCachingServices(this IHostApplicationBuilder builder)
    {
        _ = builder.Services.AddKeyedSingleton<IBaseCacheService<LookupTableCache<string>>, PayClassificationHostedService>(nameof(PayClassificationHostedService));
        _ = builder.Services.AddKeyedSingleton<IBaseCacheService<LookupTableCache<byte>>, DepartmentHostedService>(nameof(DepartmentHostedService));

        _ = builder.Services.AddDistributedMemoryCache();

        if (!builder.Environment.IsTestEnvironment())
        {
            _ = builder.Services.AddHostedService<PayClassificationHostedService>();
            _ = builder.Services.AddHostedService<DepartmentHostedService>();
        }

        return builder;
    }
}
