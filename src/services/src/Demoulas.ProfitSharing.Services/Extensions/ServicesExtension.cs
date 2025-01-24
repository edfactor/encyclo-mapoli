using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Services.Caching.Extensions;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Mappers;
using Demoulas.ProfitSharing.Services.ProfitMaster;
using Demoulas.ProfitSharing.Services.ProfitShareEdit;
using Demoulas.ProfitSharing.Services.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services.Reports;
using Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        _ = builder.Services.AddScoped<IYearEndService, YearEndService>();

        _ = builder.Services.AddScoped<TotalService>();
        _ = builder.Services.AddScoped<ContributionService>();

        _ = builder.Services.AddScoped<ITerminatedEmployeeAndBeneficiaryReportService, TerminatedEmployeeAndBeneficiaryReportService>();

        _ = builder.Services.AddSingleton<IFrozenService, FrozenService>();
        _ = builder.Services.AddSingleton<IStoreService, StoreService>();
        _ = builder.Services.AddSingleton<IAccountingPeriodsService, AccountingPeriodsService>();
        _ = builder.Services.AddSingleton<ICalendarService, CalendarService>();

        _ = builder.Services.AddScoped<IProfitShareUpdateService, ProfitShareUpdateService>();
        _ = builder.Services.AddScoped<IInternalProfitShareUpdateService, ProfitShareUpdateService>();
        _ = builder.Services.AddScoped<IProfitShareEditService, ProfitShareEditService>();
        _ = builder.Services.AddScoped<IProfitMasterService, ProfitMasterService>();

        _ = builder.Services.AddSingleton<IPayProfitUpdateService, PayProfitUpdateService>();

        #region Mappers

       
        builder.Services.AddSingleton<BeneficiaryTypeMapper>();
        builder.Services.AddSingleton<EmployeeTypeMapper>();

        #endregion

        builder.AddProjectCachingServices();


        return builder;
    }
}
