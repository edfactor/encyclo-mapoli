using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Validators;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Services.Caching.Extensions;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Mappers;
using Demoulas.ProfitSharing.Services.Military;
using Demoulas.ProfitSharing.Services.Navigations;
using Demoulas.ProfitSharing.Services.ProfitMaster;
using Demoulas.ProfitSharing.Services.ProfitShareEdit;
using Demoulas.ProfitSharing.Services.Reports;
using Demoulas.ProfitSharing.Services.Reports.Breakdown;
using Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;
using FluentValidation;
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
        _ = builder.Services.AddScoped<ITerminationAndRehireService, TerminationAndRehireService>();
        _ = builder.Services.AddScoped<ITotalService, TotalService>();
        _ = builder.Services.AddScoped<IWagesService, WagesService>();
        _ = builder.Services.AddScoped<IYearEndService, YearEndService>();
        _ = builder.Services.AddScoped<IMilitaryService, MilitaryService>();
        _ = builder.Services.AddScoped<IFakeSsnService, FakeSsnService>();

        _ = builder.Services.AddScoped<TotalService>();
        _ = builder.Services.AddScoped<ContributionService>();

        _ = builder.Services.AddScoped<ITerminatedEmployeeAndBeneficiaryReportService, TerminatedEmployeeAndBeneficiaryReportService>();

        _ = builder.Services.AddScoped<IFrozenService, FrozenService>();
        _ = builder.Services.AddScoped<IStoreService, StoreService>();
        _ = builder.Services.AddScoped<IAccountingPeriodsService, AccountingPeriodsService>();
        _ = builder.Services.AddScoped<ICalendarService, CalendarService>();

        _ = builder.Services.AddScoped<IProfitShareUpdateService, ProfitShareUpdateService>();
        _ = builder.Services.AddScoped<IInternalProfitShareUpdateService, ProfitShareUpdateService>();
        _ = builder.Services.AddScoped<IProfitShareEditService, ProfitShareEditService>();
        _ = builder.Services.AddScoped<IInternalProfitShareEditService, ProfitShareEditService>();
        _ = builder.Services.AddScoped<IProfitMasterService, ProfitMasterService>();
        _ = builder.Services.AddScoped<IPostFrozenService, PostFrozenService>();

        _ = builder.Services.AddScoped<IPayProfitUpdateService, PayProfitUpdateService>();
        _ = builder.Services.AddScoped<IBreakdownService, BreakdownReportService>();
        _ = builder.Services.AddScoped<INavigationService, NavigationService>();


        #region Mappers


        builder.Services.AddSingleton<BeneficiaryTypeMapper>();
        builder.Services.AddSingleton<EmployeeTypeMapper>();

        #endregion

        builder.AddProjectCachingServices();


        return builder;
    }
}
