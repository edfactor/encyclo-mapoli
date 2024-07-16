using Demoulas.ProfitSharing.Client.Common;
using Demoulas.ProfitSharing.Client.Reports.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Client.Extensions;

public static class RegistrationExtension
{
    public static IServiceCollection AddProfitSharingClientServices(this IServiceCollection services)
    {
        _ = services.AddScoped<IPayClassificationService, PayClassificationClient>();
        _ = services.AddScoped<IDemographicsService, DemographicsClient>();
        _ = services.AddScoped<IYearEndService, YearEndClient>();

        _ = services.AddHttpClient(Constants.Http.HttpClient).AddStandardResilienceHandler();
        _ = services.AddHttpClient(Constants.Http.ReportsHttpClient).AddStandardResilienceHandler();
        _ = services.AddHttpClient(Constants.Http.ReportsDownloadClient, client =>
        {
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/csv"));
        }).AddStandardResilienceHandler();


        return services;
    }
}
