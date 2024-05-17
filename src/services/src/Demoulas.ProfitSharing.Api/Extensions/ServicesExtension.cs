using Demoulas.ProfitSharing.Api.Utilities;

namespace Demoulas.ProfitSharing.Api.Extensions;

/// <summary>
/// Provides helper methods for configuring project services.
/// </summary>
public static class ServicesExtension
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<AppVersionInfo>();
      
        return services;
    }
}
