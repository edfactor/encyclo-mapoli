using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Api.Utilities;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.HostedServices;
using Demoulas.ProfitSharing.Services.InternalEntities;

namespace Demoulas.ProfitSharing.Api.Extensions;

/// <summary>
/// Provides helper methods for configuring project services.
/// </summary>
public static class ServicesExtension
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<AppVersionInfo>();
        _ = services.AddSingleton<IBaseCacheService<PayClassificationResponseCache>, PayClassificationHostedService>();
        
        _ = services.AddScoped<PayClassificationService>();

        return services;
    }
}
