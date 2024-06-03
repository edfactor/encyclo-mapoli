using Demoulas.ProfitSharing.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Client.Extensions;

public static class RegistrationExtension
{
    public static IServiceCollection AddProfitSharingClientServices(this IServiceCollection services)
    {
        _ = services.AddScoped<IPayClassificationService, PayClassificationClient>();
        _ = services.AddScoped<IDemographicsService, DemographicsClient>();


        services.AddHttpClient(Constants.HttpClient) .AddStandardResilienceHandler();

        return services;
    }
}
