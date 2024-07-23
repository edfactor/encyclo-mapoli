using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Api.Utilities;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.HostedServices;
using Demoulas.ProfitSharing.Services.InternalEntities;
using Demoulas.ProfitSharing.Services.Mappers;
using Demoulas.ProfitSharing.Services.Reports;

namespace Demoulas.ProfitSharing.Api.Extensions;

/// <summary>
/// Provides helper methods for configuring project services.
/// </summary>
public static class ServicesExtension
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<AppVersionInfo>();

        _ = Services.Extensions.ServicesExtension.AddProjectServices(services);

        return services;
    }
}
