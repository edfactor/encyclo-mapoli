using Demoulas.ProfitSharing.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Client.Extensions;

public static class RegistrationExtension
{
    public static IServiceCollection AddProfitSharingClientServices(this IServiceCollection services, Uri? baseAddress )
    {
        _ = services.AddScoped<IPayClassificationService, PayClassificationClient>();


        services.AddHttpClient(Constants.HttpClient, (client =>
        {
            client.BaseAddress = baseAddress;
        })) .AddStandardResilienceHandler();

        return services;
    }
}
