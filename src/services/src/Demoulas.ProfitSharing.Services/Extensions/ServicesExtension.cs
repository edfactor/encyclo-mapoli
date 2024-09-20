using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services.HostedServices;
using Demoulas.ProfitSharing.Services.Mappers;
using Demoulas.ProfitSharing.Services.Reports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Caching;
using Demoulas.ProfitSharing.OracleHcm.Extensions;

namespace Demoulas.ProfitSharing.Services.Extensions;

/// <summary>
/// Provides helper methods for configuring project builder.Services.
/// </summary>
public static class ServicesExtension
{
    public static IHostApplicationBuilder AddProjectServices(this IHostApplicationBuilder builder)
    {
        
        _ = builder.Services.AddScoped<IPayClassificationService, PayClassificationService>();
        _ = builder.Services.AddScoped<IYearEndService, CleanupReportService>();
        _ = builder.Services.AddScoped<IMilitaryAndRehireService, MilitaryAndRehireService>();
        _ = builder.Services.AddScoped<IWagesService, WagesService>();

        _ = builder.Services.AddSingleton<IDemographicsServiceInternal, DemographicsService>();
        _ = builder.Services.AddSingleton<IStoreService, StoreService>();
        _ = builder.Services.AddSingleton<CalendarService>();
        

       
        _ = builder.Services.AddKeyedSingleton<IBaseCacheService<LookupTableCache<byte>>, PayClassificationHostedService>(nameof(PayClassificationHostedService));
        _ = builder.Services.AddKeyedSingleton<IBaseCacheService<LookupTableCache<byte>>, DepartmentHostedService>(nameof(DepartmentHostedService));


        _ = builder.ConfigureMassTransitServices();
        _ = builder.ConfigureOracleHcm();

        #region Mappers

        builder.Services.AddSingleton<AddressMapper>();
        builder.Services.AddSingleton<ContactInfoMapper>();
        builder.Services.AddSingleton<DemographicMapper>();
        builder.Services.AddSingleton<TerminationCodeMapper>();
        builder.Services.AddSingleton<PayFrequencyMapper>();
        builder.Services.AddSingleton<GenderMapper>();
        builder.Services.AddSingleton<EmploymentTypeMapper>();
        builder.Services.AddSingleton<DepartmentMapper>();
        builder.Services.AddSingleton<ZeroContributionReasonMapper>();
        builder.Services.AddSingleton<BeneficiaryTypeMapper>();
        builder.Services.AddSingleton<EmployeeTypeMapper>();

        #endregion

        return builder;
    }
}
