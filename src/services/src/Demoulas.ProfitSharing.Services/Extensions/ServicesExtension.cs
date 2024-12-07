using Demoulas.Common.Caching.Interfaces;
using Demoulas.Common.Contracts.Interfaces;
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
using Demoulas.ProfitSharing.Services.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

namespace Demoulas.ProfitSharing.Services.Extensions;

/// <summary>
/// Provides helper methods for configuring project builder.Services.
/// </summary>
public static class ServicesExtension
{
    public static IHostApplicationBuilder AddProjectServices(this IHostApplicationBuilder builder)
    {
        _ = builder.Services.AddScoped<IPayClassificationService, PayClassificationService>();
        _ = builder.Services.AddScoped<ICleanupReportService, CleanupReportService>();
        _ = builder.Services.AddScoped<IFrozenReportService, FrozenReportService>();
        _ = builder.Services.AddScoped<IMasterInquiryService, MasterInquiryService>();
        _ = builder.Services.AddScoped<IExecutiveHoursAndDollarsService, ExecutiveHoursAndDollarsService>();
        _ = builder.Services.AddScoped<IGetEligibleEmployeesService, GetEligibleEmployeesService>();
        _ = builder.Services.AddScoped<IMilitaryAndRehireService, MilitaryAndRehireService>();
        _ = builder.Services.AddScoped<ITotalService, TotalService>();
        _ = builder.Services.AddScoped<IWagesService, WagesService>();

        _ = builder.Services.AddScoped<TotalService>();
        _ = builder.Services.AddScoped<ContributionService>();
        
        _ = builder.Services.AddScoped<ITerminatedEmployeeAndBeneficiaryReportService, TerminatedEmployeeAndBeneficiaryReportService>();

        _ = builder.Services.AddSingleton<IDemographicsServiceInternal, DemographicsService>();
        _ = builder.Services.AddSingleton<IStoreService, StoreService>();
        _ = builder.Services.AddSingleton<IAccountingPeriodsService, AccountingPeriodsService>();
        _ = builder.Services.AddSingleton<ICalendarService, CalendarService>();

        _ = builder.Services.AddSingleton<IProfitShareUpdateService, ProfitShareUpdateService>();


        _ = builder.Services.AddKeyedSingleton<IBaseCacheService<LookupTableCache<byte>>, PayClassificationHostedService>(nameof(PayClassificationHostedService));
        _ = builder.Services.AddKeyedSingleton<IBaseCacheService<LookupTableCache<byte>>, DepartmentHostedService>(nameof(DepartmentHostedService));


        _ = builder.ConfigureOracleHcm();

        #region Mappers

        builder.Services.AddSingleton<AddressMapper>();
        builder.Services.AddSingleton<ContactInfoMapper>();
        builder.Services.AddSingleton<DemographicMapper>();
        builder.Services.AddSingleton<BeneficiaryTypeMapper>();
        builder.Services.AddSingleton<EmployeeTypeMapper>();

        #endregion

        builder.ConfigureMassTransitServices();
        
        return builder;
    }
}
