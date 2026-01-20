using System.Text.Json;
using Demoulas.Common.Contracts.Interfaces.Audit;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.CheckRun;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Common.Time;
using Demoulas.ProfitSharing.Services.Administration;
using Demoulas.ProfitSharing.Services.Audit;
using Demoulas.ProfitSharing.Services.Beneficiaries;
using Demoulas.ProfitSharing.Services.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Services.Caching.Extensions;
using Demoulas.ProfitSharing.Services.Caching.HostedServices;
using Demoulas.ProfitSharing.Services.Certificates;
using Demoulas.ProfitSharing.Services.CheckRun;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.ItDevOps;
using Demoulas.ProfitSharing.Services.Lookup;
using Demoulas.ProfitSharing.Services.MasterInquiry;
using Demoulas.ProfitSharing.Services.MergeProfitDetails;
using Demoulas.ProfitSharing.Services.Military;
using Demoulas.ProfitSharing.Services.Navigations;
using Demoulas.ProfitSharing.Services.PrintFormatting;
using Demoulas.ProfitSharing.Services.ProfitMaster;
using Demoulas.ProfitSharing.Services.ProfitShareEdit;
using Demoulas.ProfitSharing.Services.Reports;
using Demoulas.ProfitSharing.Services.Reports.Breakdown;
using Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;
using Demoulas.ProfitSharing.Services.Serialization;
using Demoulas.ProfitSharing.Services.Validation;
using Demoulas.ProfitSharing.Services.Validators;
using Demoulas.Util.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using INavigationService = Demoulas.Common.Contracts.Interfaces.INavigationService;
using NavigationService = Demoulas.ProfitSharing.Services.Navigations.ProfitSharingNavigationService;

namespace Demoulas.ProfitSharing.Services.Extensions;

/// <summary>
/// Provides helper methods for configuring project builder.Services.
/// </summary>
public static class ServicesExtension
{
    public static IHostApplicationBuilder AddProjectServices(this IHostApplicationBuilder builder)
    {
        _ = builder.Services.AddSingleton<JsonSerializerOptions>(sp =>
        {
            IHostEnvironment hostEnvironment = sp.GetRequiredService<IHostEnvironment>();
            var maskingOptions = new JsonSerializerOptions(JsonSerializerOptions.Web);
            maskingOptions.Converters.Add(new MaskingJsonConverterFactory(hostEnvironment));

            return maskingOptions;
        });

        _ = builder.Services.AddScoped<IPayClassificationService, PayClassificationService>();
        _ = builder.Services.AddScoped<ICertificateService, CertificateService>();
        _ = builder.Services.AddScoped<ICheckRunWorkflowService, CheckRunWorkflowService>();
        _ = builder.Services.AddScoped<ICheckRunOrchestrator, CheckRunOrchestrator>();
        _ = builder.Services.AddScoped<ICheckRunPrintFileService, CheckRunPrintFileService>();

        _ = builder.Services.AddSingleton<IMicrFormatterFactory, MicrFormatterFactory>();
        _ = builder.Services.AddSingleton<DjdeFormatBuilder>();
        _ = builder.Services.AddScoped<ICleanupReportService, CleanupReportService>();
        _ = builder.Services.AddScoped<IDuplicateNamesAndBirthdaysService, DuplicateNamesAndBirthdaysService>();
        _ = builder.Services.AddScoped<IDistributionService, DistributionService>();
        _ = builder.Services.AddScoped<IEmbeddedSqlService, EmbeddedSqlService>();
        _ = builder.Services.AddScoped<IForfeituresAndPointsForYearService, ForfeituresAndPointsForYearService>();
        _ = builder.Services.AddScoped<IFrozenReportService, FrozenReportService>();
        _ = builder.Services.AddScoped<IMasterInquiryService, MasterInquiryService>();
        _ = builder.Services.AddScoped<IEmployeeMasterInquiryService, EmployeeMasterInquiryService>();
        _ = builder.Services.AddScoped<IBeneficiaryMasterInquiryService, BeneficiaryMasterInquiryService>();
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
        _ = builder.Services.AddScoped<IEmployeesWithProfitsOver73Service, EmployeesWithProfitsOver73Service>();


        _ = builder.Services.AddScoped<IProfitSharingAuditService, ProfitSharingAuditService>();
        _ = builder.Services.AddScoped<IAuditService>(sp => sp.GetRequiredService<IProfitSharingAuditService>());

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
        _ = builder.Services.AddScoped<IProfitDetailReversalsService, ProfitDetailReversalsService>();
        _ = builder.Services.AddScoped<IProfitSharingAdjustmentsService, ProfitSharingAdjustmentsService>();
        _ = builder.Services.AddScoped<IBreakdownService, BreakdownReportService>();
        _ = builder.Services.AddScoped<INavigationService, NavigationService>();
        _ = builder.Services.AddScoped<IBeneficiaryInquiryService, BeneficiaryInquiryService>();

        _ = builder.Services.AddScoped<INavigationPrerequisiteValidator, NavigationPrerequisiteValidator>();


        _ = builder.Services.AddScoped<ITableMetadataService, TableMetadataService>();
        _ = builder.Services.AddScoped<IOracleHcmDiagnosticsService, OracleHcmDiagnosticsService>();
        _ = builder.Services.AddScoped<IStateTaxRatesService, StateTaxRatesService>();
        _ = builder.Services.AddScoped<IAnnuityRatesService, AnnuityRatesService>();
        _ = builder.Services.AddScoped<IAnnuityRateValidator, AnnuityRateValidator>();
        _ = builder.Services.AddScoped<IRmdsFactorService, RmdsFactorService>();
        _ = builder.Services.AddScoped<ICommentTypeService, CommentTypeService>();
        _ = builder.Services.AddScoped<ITaxCodeService, TaxCodeService>();
        _ = builder.Services.AddScoped<IBankService, BankService>();
        _ = builder.Services.AddScoped<IBankAccountService, BankAccountService>();
        _ = builder.Services.AddSingleton<IVestingScheduleService, VestingScheduleService>();

        _ = builder.Services.AddScoped<IDemographicReaderService, DemographicReaderService>();
        _ = builder.Services.AddScoped<IUnmaskingService, UnmaskingService>();
        _ = builder.Services.AddScoped<IPayBenReportService, PayBenReportService>();
        _ = builder.Services.AddScoped<IAccountHistoryReportService, AccountHistoryReportService>();

        _ = builder.Services.AddScoped<IReportRunnerService, ReportRunnerService>();
        _ = builder.Services.AddScoped<IStateTaxLookupService, StateTaxLookupService>();
        _ = builder.Services.AddScoped<ICommentTypeLookupService, CommentTypeLookupService>();
        _ = builder.Services.AddScoped<IDistributionFrequencyLookupService, DistributionFrequencyLookupService>();
        _ = builder.Services.AddScoped<IDistributionStatusLookupService, DistributionStatusLookupService>();
        _ = builder.Services.AddScoped<ITaxCodeLookupService, TaxCodeLookupService>();
        _ = builder.Services.AddScoped<IStateService, StateService>();
        _ = builder.Services.AddScoped<IMergeProfitDetailsService, MergeProfitDetailsService>();
        _ = builder.Services.AddScoped<IBeneficiaryDisbursementService, BeneficiaryDisbursementService>();

        // Validation services
        _ = builder.Services.AddScoped<IChecksumValidationService, ChecksumValidationService>();
        _ = builder.Services.AddScoped<ICrossReferenceValidationService, CrossReferenceValidationService>();
        _ = builder.Services.AddScoped<IArchivedValueService, ArchivedValueService>();
        _ = builder.Services.AddScoped<IAllocTransferValidationService, AllocTransferValidationService>();
        _ = builder.Services.AddScoped<IBalanceEquationValidationService, BalanceEquationValidationService>();

        // Register lookup caches as singletons (they manage their own distributed cache access)
        _ = builder.Services.AddSingleton<Services.Caching.StateTaxCache>();
        _ = builder.Services.AddSingleton<Services.Caching.ProfitCodeCache>();

        builder.AddProjectCachingServices();

        // Register TimeProvider (fake or real based on configuration and environment)
        builder.AddTimeProvider();

        // Register cache warmer hosted service (not in test environment)
        if (!builder.Environment.IsTestEnvironment())
        {
            _ = builder.Services.AddHostedService<StateTaxCacheWarmerHostedService>();
        }

        return builder;
    }

    /// <summary>
    /// Registers the appropriate <see cref="TimeProvider"/> based on configuration and environment.
    /// SECURITY: Fake time is only enabled in non-production environments when explicitly configured.
    /// In non-production environments, a SwitchableTimeProvider is registered to allow runtime toggling.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <returns>The builder for chaining.</returns>
    public static IHostApplicationBuilder AddTimeProvider(this IHostApplicationBuilder builder)
    {
        var fakeTimeConfig = builder.Configuration
            .GetSection(FakeTimeConfiguration.SectionName)
            .Get<FakeTimeConfiguration>() ?? new FakeTimeConfiguration();

        // SECURITY: Never allow fake time in Production
        var isProduction = builder.Environment.IsProduction();

        if (isProduction)
        {
            // Production: Use the real system time provider (no switching allowed)
            _ = builder.Services.AddSingleton(TimeProvider.System);

            // Register a disabled configuration for status reporting
            _ = builder.Services.AddSingleton(new FakeTimeConfiguration { Enabled = false });

            if (fakeTimeConfig.Enabled)
            {
                // Log a warning that fake time was requested but disabled in production
                // This uses a temporary logger since DI isn't fully built yet
                Console.WriteLine(
                    "WARNING: FakeTime.Enabled is true but this is a Production environment. " +
                    "Fake time has been disabled for security reasons.");
            }
        }
        else
        {
            // Non-production: Use SwitchableTimeProvider to allow runtime toggling
            if (fakeTimeConfig.Enabled)
            {
                // Validate configuration if fake time is pre-configured
                var validationErrors = fakeTimeConfig.Validate();
                if (validationErrors.Count > 0)
                {
                    throw new InvalidOperationException(
                        $"FakeTime configuration is invalid: {string.Join("; ", validationErrors)}");
                }
            }

            // Register the switchable provider - it will start with fake time if configured
            _ = builder.Services.AddSingleton<SwitchableTimeProvider>(sp =>
            {
                var logger = sp.GetService<ILogger<SwitchableTimeProvider>>();
                return fakeTimeConfig.Enabled
                    ? new SwitchableTimeProvider(fakeTimeConfig, logger)
                    : new SwitchableTimeProvider(logger);
            });

            // Register TimeProvider interface to resolve to the same instance
            _ = builder.Services.AddSingleton<TimeProvider>(sp => sp.GetRequiredService<SwitchableTimeProvider>());

            // Register the initial configuration for status reporting
            _ = builder.Services.AddSingleton(fakeTimeConfig);

            // Register per-user fake time services (only available in non-production)
            _ = builder.Services.AddSingleton<IUserFakeTimeStorage, UserFakeTimeStorage>();
            _ = builder.Services.AddScoped<IUserTimeService, UserTimeService>();
        }

        return builder;
    }
}
