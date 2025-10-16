using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Channels;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.OracleHcm.Clients;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Factories;
using Demoulas.ProfitSharing.OracleHcm.HealthCheck;
using Demoulas.ProfitSharing.OracleHcm.HostedServices;
using Demoulas.ProfitSharing.OracleHcm.Jobs;
using Demoulas.ProfitSharing.OracleHcm.Messaging;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Demoulas.ProfitSharing.OracleHcm.Validators;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.BeneficiaryInquiry;
using Demoulas.ProfitSharing.Services.Caching.Extensions;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.ItDevOps;
using Demoulas.Util.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using OpenTelemetry.Trace;
using Polly;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using AddressMapper = Demoulas.ProfitSharing.OracleHcm.Mappers.AddressMapper;
using ContactInfoMapper = Demoulas.ProfitSharing.OracleHcm.Mappers.ContactInfoMapper;
using DemographicMapper = Demoulas.ProfitSharing.OracleHcm.Mappers.DemographicMapper;

namespace Demoulas.ProfitSharing.OracleHcm.Extensions;

public static class OracleHcmExtension
{
    private sealed class HttpResilienceOptions
    {
        public required HttpCircuitBreakerStrategyOptions CircuitBreakerOptions { get; init; }
        public required HttpTimeoutStrategyOptions AttemptTimeoutOptions { get; init; }
        public required HttpTimeoutStrategyOptions TotalRequestTimeoutOptions { get; init; }
    }

    private const string FrameworkVersionHeader = "REST-Framework-Version";

    /// <summary>
    /// Adds the Oracle HCM background process to the application builder.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> to configure.</param>
    /// <returns>The configured <see cref="IHostApplicationBuilder"/>.</returns>
    /// <remarks>
    /// This method registers the Oracle HCM synchronization services and the hosted service
    /// responsible for managing Oracle HCM background processes. It ensures that the necessary
    /// dependencies and configurations are added to the application, including a null implementation
    /// of INavigationService suitable for console/background service contexts.
    /// </remarks>
#if DEBUG
    public static IHostApplicationBuilder AddEmployeeDeltaSyncService(this IHostApplicationBuilder builder, ISet<long>? debugOracleHcmIdSet = null)
#else
    public static IHostApplicationBuilder AddEmployeeDeltaSyncService(this IHostApplicationBuilder builder)
#endif
    {
        OracleHcmConfig oracleHcmConfig = builder.Configuration.GetSection("OracleHcm").Get<OracleHcmConfig>()
                                          ?? new OracleHcmConfig { BaseAddress = string.Empty, DemographicUrl = string.Empty };

        // Process each delta employee one at a time.
        oracleHcmConfig.Limit = 1;

        // Register null navigation service for console app context (navigation concepts don't apply)
        builder.Services.AddScoped<INavigationService, NullNavigationService>();

        // Add Oracle HCM synchronization with the retrieved configuration.
        builder.AddOracleHcmSynchronization(oracleHcmConfig);

#if DEBUG
        builder.Services.AddTransient((_) => debugOracleHcmIdSet ?? new HashSet<long>());
#endif

        builder.Services.AddHostedService<EmployeeDeltaSyncService>();

        builder.Services.AddScoped<ITotalService, TotalService>();
        builder.Services.AddSingleton<ICalendarService, CalendarService>();
        builder.Services.AddScoped<ITotalService, TotalService>();
        builder.Services.AddSingleton<IAccountingPeriodsService, AccountingPeriodsService>();
        builder.Services.AddScoped<IEmbeddedSqlService, EmbeddedSqlService>();
        builder.Services.AddScoped<IDemographicReaderService, DemographicReaderService>();
        builder.Services.AddScoped<IFrozenService, FrozenService>();
        builder.Services.AddScoped<IBeneficiaryInquiryService, BeneficiaryInquiryService>();

        return builder;
    }

    public static IHostApplicationBuilder AddEmployeeFullSyncService(this IHostApplicationBuilder builder)
    {
        OracleHcmConfig oracleHcmConfig = builder.Configuration.GetSection("OracleHcm").Get<OracleHcmConfig>()
                                          ?? new OracleHcmConfig { BaseAddress = string.Empty, DemographicUrl = string.Empty };

        // Register null navigation service for console app context (navigation concepts don't apply)
        builder.Services.AddScoped<INavigationService, NullNavigationService>();

        builder.Services.AddScoped<ITotalService, TotalService>();
        builder.Services.AddSingleton<ICalendarService, CalendarService>();
        builder.Services.AddScoped<ITotalService, TotalService>();
        builder.Services.AddSingleton<IAccountingPeriodsService, AccountingPeriodsService>();
        builder.Services.AddScoped<IEmbeddedSqlService, EmbeddedSqlService>();
        builder.Services.AddScoped<IDemographicReaderService, DemographicReaderService>();
        builder.Services.AddScoped<IFrozenService, FrozenService>();

        builder.AddOracleHcmSynchronization(oracleHcmConfig);
        builder.Services.AddHostedService<EmployeeFullSyncService>();
        return builder;
    }

    public static IHostApplicationBuilder AddEmployeePayrollSyncService(this IHostApplicationBuilder builder)
    {
        OracleHcmConfig oracleHcmConfig = builder.Configuration.GetSection("OracleHcm").Get<OracleHcmConfig>()
                                          ?? new OracleHcmConfig { BaseAddress = string.Empty, DemographicUrl = string.Empty };

        // Register null navigation service for console app context (navigation concepts don't apply)
        builder.Services.AddScoped<INavigationService, NullNavigationService>();

        builder.AddOracleHcmSynchronization(oracleHcmConfig);
        builder.Services.AddHostedService<EmployeePayrollSyncService>();

        builder.Services.AddScoped<ITotalService, TotalService>();
        builder.Services.AddSingleton<ICalendarService, CalendarService>();
        builder.Services.AddScoped<ITotalService, TotalService>();
        builder.Services.AddSingleton<IAccountingPeriodsService, AccountingPeriodsService>();
        builder.Services.AddScoped<IEmbeddedSqlService, EmbeddedSqlService>();
        builder.Services.AddScoped<IDemographicReaderService, DemographicReaderService>();
        builder.Services.AddScoped<IFrozenService, FrozenService>();

        return builder;
    }


    /// <summary>
    /// Configures and registers services required for Oracle HCM synchronization.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> used to configure the application.</param>
    /// <param name="oracleHcmConfig"></param>
    /// <returns>The updated <see cref="IHostApplicationBuilder"/> instance.</returns>
    /// <remarks>
    /// This method performs the following actions:
    /// - Initializes endpoint telemetry for business operations tracking
    /// - Retrieves and binds the Oracle HCM configuration.
    /// - Registers the Oracle HCM configuration as a singleton service.
    /// - Registers necessary Oracle HCM services.
    /// - Configures HTTP clients for Oracle HCM.
    /// - Adds a health check for Oracle HCM.
    /// - Adds project-specific caching services.
    /// - Configures Oracle HCM messaging.
    /// </remarks>
    public static IHostApplicationBuilder AddOracleHcmSynchronization(this IHostApplicationBuilder builder,
        OracleHcmConfig oracleHcmConfig)
    {
        // Initialize endpoint telemetry for comprehensive business operations tracking
        // Safe to call multiple times (idempotent)
        EndpointTelemetry.Initialize();

        builder.Services.AddTransient((_) => oracleHcmConfig);

        RegisterOracleHcmServices(builder.Services);
        ConfigureHttpClients(builder.Services);

        builder.Services.AddHealthChecks().AddCheck<OracleHcmHealthCheck>("OracleHcm")
            .AddProcessAllocatedMemoryHealthCheck(maximumMegabytesAllocated: 1024);
        builder.AddProjectCachingServices();
        builder.AddOracleHcmMessaging();

        builder.Services.AddOpenTelemetry().WithTracing(tracing =>
        {
            tracing.AddQuartzInstrumentation();
        });

        if (!builder.Environment.IsTestEnvironment())
        {
            builder.Services.AddHostedService<MemoryMonitoringService>();
        }

        return builder;
    }

    /// <summary>
    /// Registers the necessary services for Oracle HCM integration into the provided service collection.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the Oracle HCM services will be added.
    /// </param>
    /// <remarks>
    /// This method configures general services, mappers, and internal services required for Oracle HCM functionality.
    /// It ensures that dependencies such as validators, jobs, and mappers are properly registered.
    /// </remarks>
    private static void RegisterOracleHcmServices(IServiceCollection services)
    {
        // General services
        services.AddTransient<OracleEmployeeValidator>();
        services.AddTransient<EmployeeFullSyncJob>();
        services.AddTransient<EmployeeDeltaSyncJob>();
        services.AddTransient<PayrollSyncJob>();
        services.AddTransient<DemographicsService>();

        // Mappers
        services.AddTransient<DemographicMapper>();
        services.AddTransient<AddressMapper>();
        services.AddTransient<ContactInfoMapper>();


        // Internal services
        services.AddTransient<IDemographicsServiceInternal, DemographicsService>();
        services.AddTransient<IEmployeeSyncService, EmployeeSyncService>();
        services.AddTransient<IJobFactory, OracleHcmJobFactory>();
        services.AddTransient<ISchedulerFactory, StdSchedulerFactory>();

        services.AddSingleton<IFakeSsnService, FakeSsnService>();

    }

    /// <summary>
    /// Configures HTTP clients used for Oracle HCM integration by registering them with resilience strategies.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the HTTP clients will be added.
    /// </param>
    /// <remarks>
    /// This method sets up HTTP clients for various Oracle HCM services, such as Atom feed synchronization,
    /// employee synchronization, and payroll synchronization. It applies standard resilience strategies,
    /// including circuit breakers and timeout configurations, to ensure robust communication with the Oracle HCM API.
    /// </remarks>
    private static void ConfigureHttpClients(IServiceCollection services)
    {
        HttpResilienceOptions commonHttpOptions = new HttpResilienceOptions
        {
            CircuitBreakerOptions = new HttpCircuitBreakerStrategyOptions
            {
                SamplingDuration = TimeSpan.FromMinutes(4),
                MinimumThroughput = 10, // Add minimum throughput
                FailureRatio = 0.5 // Add failure ratio
            },
            AttemptTimeoutOptions = new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromMinutes(2)
            },
            TotalRequestTimeoutOptions = new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromMinutes(5)
            }
        };

        // Custom retry handler for 401 Unauthorized, with jitter. Bulkhead is not available directly in this API version.
        static void AddRetryOn401WithJitter(HttpStandardResilienceOptions options)
        {
            options.Retry.ShouldHandle = args =>
            {
                if (args.Outcome.Result is { StatusCode: System.Net.HttpStatusCode.Unauthorized })
                {
                    // Optionally log here
                    return ValueTask.FromResult(true);
                }
                return ValueTask.FromResult(false);
            };
            options.Retry.MaxRetryAttempts = 3;
            options.Retry.BackoffType = DelayBackoffType.Exponential;
            options.Retry.Delay = TimeSpan.FromMinutes(4); // was 2

            // Add jitter to retry delay (doubled base delay & exponential growth factor)
            var random = new Random();
            options.Retry.DelayGenerator = args =>
            {
                // base delay doubled (2s -> 4s)
                var baseDelay = TimeSpan.FromSeconds(4);
                // exponential component doubled by shifting power (+1) relative to previous implementation
                var exponential = TimeSpan.FromMinutes(Math.Pow(2, args.AttemptNumber + 1));
                var jitter = TimeSpan.FromMilliseconds(random.Next(0, 1000));
                return new ValueTask<TimeSpan?>(baseDelay + exponential + jitter);
            };
        }

        void ConfigureWith401RetryAndBulkhead(HttpStandardResilienceOptions options, HttpResilienceOptions commonOptions)
        {
            ApplyResilienceOptions(options, commonOptions);
            if (!Debugger.IsAttached)
            {
                AddRetryOn401WithJitter(options);
            }
        }

        services.AddHttpClient<AtomFeedClient>("AtomFeedSync", BuildOracleHcmAuthClient)
            .AddStandardResilienceHandler(options => ConfigureWith401RetryAndBulkhead(options, commonHttpOptions));

        services.AddHttpClient<EmployeeFullSyncClient>("EmployeeSync", BuildOracleHcmAuthClient)
            .AddStandardResilienceHandler(options => ConfigureWith401RetryAndBulkhead(options, commonHttpOptions));

        services.AddHttpClient<PayrollSyncClient>("PayrollSyncClient", BuildOracleHcmAuthClient)
            .AddStandardResilienceHandler(options => ConfigureWith401RetryAndBulkhead(options, commonHttpOptions));

        services.AddHttpClient<PayrollSyncService>("PayrollSyncService", BuildOracleHcmAuthClient)
            .AddStandardResilienceHandler(options => ConfigureWith401RetryAndBulkhead(options, commonHttpOptions));

        services.AddHttpClient<OracleHcmHealthCheck>("OracleHcmHealthCheck", BuildOracleHcmAuthClient)
            .AddStandardResilienceHandler(options => ApplyResilienceOptions(options, commonHttpOptions));
    }

    /// <summary>
    /// Configures resilience options for HTTP requests.
    /// </summary>
    /// <param name="options">The <see cref="HttpStandardResilienceOptions"/> to configure.</param>
    /// <param name="commonOptions">
    /// The common resilience options containing configurations for circuit breaker, 
    /// attempt timeout, and total request timeout strategies.
    /// </param>
    /// <remarks>
    /// This method applies the specified resilience strategies to the provided HTTP options, 
    /// ensuring consistent handling of circuit breaker, attempt timeout, and total request timeout 
    /// across HTTP clients.
    /// </remarks>
    private static void ApplyResilienceOptions(HttpStandardResilienceOptions options, HttpResilienceOptions commonOptions)
    {
        options.CircuitBreaker = commonOptions.CircuitBreakerOptions;
        options.AttemptTimeout = commonOptions.AttemptTimeoutOptions;
        options.TotalRequestTimeout = commonOptions.TotalRequestTimeoutOptions;
    }


    /// <summary>
    /// Configures the messaging infrastructure for Oracle HCM synchronization.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IHostApplicationBuilder"/> used to configure the application.
    /// </param>
    /// <returns>
    /// The modified <see cref="IHostApplicationBuilder"/> with Oracle HCM messaging services configured.
    /// </returns>
    /// <remarks>
    /// This method registers bounded channels for handling Oracle HCM employee and payroll messages.
    /// It also adds hosted services for consuming these channels.
    /// </remarks>
    private static IHostApplicationBuilder AddOracleHcmMessaging(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton(Channel.CreateBounded<MessageRequest<OracleEmployee[]>>(50));
        builder.Services.AddSingleton(Channel.CreateBounded<MessageRequest<PayrollItem[]>>(50));

        builder.Services.AddHostedService<EmployeeSyncChannelConsumer>();
        builder.Services.AddHostedService<PayrollSyncChannelConsumer>();

        return builder;
    }

    /// <summary>
    /// Configures the provided <see cref="HttpClient"/> instance for Oracle HCM authentication.
    /// Sets the base address, authorization headers, and default request headers required for communication with the Oracle HCM API.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceProvider"/> instance used to resolve dependencies, including the <see cref="OracleHcmConfig"/>.
    /// </param>
    /// <param name="client">
    /// The <see cref="HttpClient"/> instance to be configured for Oracle HCM API communication.
    /// </param>
    private static void BuildOracleHcmAuthClient(IServiceProvider services, HttpClient client)
    {
        OracleHcmConfig config = services.GetRequiredService<OracleHcmConfig>();
        if (config.Username == null)
        {
            return;
        }
        string authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}"));

        client.BaseAddress = new Uri(config.BaseAddress, UriKind.Absolute);
        client.DefaultRequestHeaders.Add(FrameworkVersionHeader, config.RestFrameworkVersion);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); // Specify JSON format
    }
}
