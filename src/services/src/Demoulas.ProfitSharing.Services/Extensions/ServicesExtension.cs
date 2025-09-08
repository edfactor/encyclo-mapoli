using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Services.Audit;
using Demoulas.ProfitSharing.Services.Beneficiaries;
using Demoulas.ProfitSharing.Services.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Services.Caching.Extensions;
using Demoulas.ProfitSharing.Services.Certificates;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.ItDevOps;
using Demoulas.ProfitSharing.Services.ItOperations;
using Demoulas.ProfitSharing.Services.Mappers;
using Demoulas.ProfitSharing.Services.MasterInquiry;
using Demoulas.ProfitSharing.Services.Military;
using Demoulas.ProfitSharing.Services.Navigations;
using Demoulas.ProfitSharing.Services.ProfitMaster;
using Demoulas.ProfitSharing.Services.ProfitShareEdit;
using Demoulas.ProfitSharing.Services.Reports;
using Demoulas.ProfitSharing.Services.Reports.Breakdown;
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
        _ = builder.Services.AddScoped<ICertificateService, CertificateService>();
        _ = builder.Services.AddScoped<ICleanupReportService, CleanupReportService>();
        _ = builder.Services.AddScoped<IEmbeddedSqlService, EmbeddedSqlService>();
        _ = builder.Services.AddScoped<IForfeituresAndPointsForYearService, ForfeituresAndPointsForYearService>();
        _ = builder.Services.AddScoped<IFrozenReportService, FrozenReportService>();
        _ = builder.Services.AddScoped<IMasterInquiryService, MasterInquiryService>();
        _ = builder.Services.AddScoped<IForfeitureAdjustmentService, ForfeitureAdjustmentService>();
        _ = builder.Services.AddScoped<IExecutiveHoursAndDollarsService, ExecutiveHoursAndDollarsService>();
        _ = builder.Services.AddScoped<IGetEligibleEmployeesService, GetEligibleEmployeesService>();
        _ = builder.Services.AddScoped<IMissiveService, MissiveService>();
        _ = builder.Services.AddScoped<IUnforfeitService, UnforfeitService>();

        _ = builder.Services.AddScoped<ITotalService, TotalService>();
        _ = builder.Services.AddScoped<IWagesService, WagesService>();
        _ = builder.Services.AddScoped<IYearEndService, YearEndService>();
        _ = builder.Services.AddScoped<IMilitaryService, MilitaryService>();
        _ = builder.Services.AddScoped<IEmployeeLookupService, Lookup.EmployeeLookupService>();

        _ = builder.Services.AddScoped<IPayrollDuplicateSsnReportService, PayrollDuplicateSsnReportService>();
        _ = builder.Services.AddScoped<INegativeEtvaReportService, NegativeEtvaReportService>();
        _ = builder.Services.AddScoped<IProfitSharingSummaryReportService, ProfitSharingSummaryReportService>();
        _ = builder.Services.AddScoped<IBeneficiaryService, BeneficiaryService>();
        _ = builder.Services.AddScoped<IAdhocBeneficiariesReport, AdhocBeneficiariesReport>();
        _ = builder.Services.AddScoped<IAdhocTerminatedEmployeesService, AdhocTerminatedEmployeesService>();


        _ = builder.Services.AddScoped<IAuditService, AuditService>();
        _ = builder.Services.AddScoped<TotalService>();

        _ = builder.Services.AddScoped<ITerminatedEmployeeService, TerminatedEmployeeService>();

        _ = builder.Services.AddScoped<IFrozenService, FrozenService>();
        _ = builder.Services.AddScoped<IStoreService, StoreService>();

        _ = builder.Services.AddSingleton<IFakeSsnService, FakeSsnService>();
        _ = builder.Services.AddSingleton<IAccountingPeriodsService, AccountingPeriodsService>();
        _ = builder.Services.AddSingleton<ICalendarService, CalendarService>();

        _ = builder.Services.AddScoped<IProfitShareUpdateService, ProfitShareUpdateService>();
        _ = builder.Services.AddScoped<IInternalProfitShareUpdateService, ProfitShareUpdateService>();
        _ = builder.Services.AddScoped<IProfitShareEditService, ProfitShareEditService>();
        _ = builder.Services.AddScoped<IInternalProfitShareEditService, ProfitShareEditService>();
        _ = builder.Services.AddScoped<IProfitMasterService, ProfitMasterService>();
        _ = builder.Services.AddScoped<IPostFrozenService, PostFrozenService>();

        _ = builder.Services.AddScoped<IPayProfitUpdateService, PayProfitUpdateService>();
        _ = builder.Services.AddScoped<IBreakdownService, BreakdownReportService>();
        _ = builder.Services.AddScoped<INavigationService, NavigationService>();
        _ = builder.Services.AddScoped<IBeneficiaryInquiryService, BeneficiaryInquiryService>();

        _ = builder.Services.AddScoped<INavigationPrerequisiteValidator, NavigationPrerequisiteValidator>();


        _ = builder.Services.AddScoped<ITableMetadataService, TableMetadataService>();

        _ = builder.Services.AddScoped<IDemographicReaderService, DemographicReaderService>();
        _ = builder.Services.AddScoped<IPayBenReportService, PayBenReportService>();

        _ = builder.Services.AddScoped<IReportRunnerService, ReportRunnerService>();

        #region Mappers


        builder.Services.AddSingleton<BeneficiaryTypeMapper>();
        builder.Services.AddSingleton<EmployeeTypeMapper>();

        #endregion

        builder.AddProjectCachingServices();

        return builder;
    }
}
