using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Api.Utilities;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.HostedServices;
using Demoulas.ProfitSharing.Services.InternalEntities;
using Demoulas.ProfitSharing.Services.Mappers;

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

        _ = services.AddScoped<IPayClassificationService, PayClassificationService>();
        _ = services.AddScoped<IDemographicsService, DemographicsService>();
        _ = services.AddScoped<IYearEndService, YearEndService>();
        _ = services.AddScoped<IPayProfitService, PayProfitService>();


        #region Mappers

        services.AddSingleton<AddressMapper>();
        services.AddSingleton<ContactInfoMapper>();
        services.AddSingleton<DemographicMapper>();
        services.AddSingleton<TerminationCodeMapper>();
        services.AddSingleton<PayFrequencyMapper>();
        services.AddSingleton<GenderMapper>();
        services.AddSingleton<EmploymentTypeMapper>();
        services.AddSingleton<DepartmentMapper>();
        services.AddSingleton<PayProfitMapper>();
        services.AddSingleton<ZeroContributionReasonMapper>();
        services.AddSingleton<BeneficiaryTypeMapper>();
        services.AddSingleton<EmployeeTypeMapper>();

        #endregion

        return services;
    }
}
