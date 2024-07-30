using Demoulas.ProfitSharing.Api.Utilities;

namespace Demoulas.ProfitSharing.Api.Extensions;

/// <summary>
/// Provides helper methods for configuring project services.
/// </summary>
public static class ServicesExtension
{
    public static IHostApplicationBuilder AddProjectServices(this IHostApplicationBuilder builder)
    {
        _ = Services.Extensions.ServicesExtension.AddProjectServices(builder);

        return builder;
    }
}
