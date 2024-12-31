using System.Net.Http.Headers;
using System.Text;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.Atom;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Factories;
using Demoulas.ProfitSharing.OracleHcm.HealthCheck;
using Demoulas.ProfitSharing.OracleHcm.HostedServices;
using Demoulas.ProfitSharing.OracleHcm.Jobs;
using Demoulas.ProfitSharing.OracleHcm.Messaging;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Demoulas.ProfitSharing.OracleHcm.Validators;
using Demoulas.ProfitSharing.Services.Caching.Extensions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
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

    public static IHostApplicationBuilder AddOracleHcmBackgroundProcess(this IHostApplicationBuilder builder)
    {
        builder.AddOracleHcmSynchronization();
        builder.Services.AddHostedService<OracleHcmHostedService>();
        return builder;
    }

    public static IHostApplicationBuilder AddOracleHcmSynchronization(this IHostApplicationBuilder builder)
    {
        var oracleHcmConfig = builder.Configuration.GetSection("OracleHcm").Get<OracleHcmConfig>()
                              ?? new OracleHcmConfig { BaseAddress = string.Empty, DemographicUrl = string.Empty };
        builder.Services.AddSingleton(oracleHcmConfig);

        RegisterOracleHcmServices(builder.Services);
        ConfigureHttpClients(builder.Services);

        builder.Services.AddHealthChecks().AddCheck<OracleHcmHealthCheck>("OracleHcm");
        builder.AddProjectCachingServices();
        builder.AddOracleHcmMessaging();

        return builder;
    }

    private static void RegisterOracleHcmServices(IServiceCollection services)
    {
        // General services
        services.AddSingleton<OracleEmployeeValidator>();
        services.AddSingleton<EmployeeFullSyncJob>();
        services.AddSingleton<EmployeeDeltaSyncJob>();
        services.AddSingleton<PayrollSyncJob>();
        services.AddSingleton<DemographicsService>();
        services.AddSingleton<EmployeeMapper>();
        services.AddSingleton<SyncJobService>();

        // Mappers
        services.AddSingleton<DemographicMapper>();
        services.AddSingleton<AddressMapper>();
        services.AddSingleton<ContactInfoMapper>();

        // Internal services
        services.AddSingleton<IDemographicsServiceInternal, DemographicsService>();
        services.AddSingleton<IJobFactory, OracleHcmJobFactory>();
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
    }

    private static void ConfigureHttpClients(IServiceCollection services)
    {
        var commonHttpOptions = new HttpResilienceOptions
        {
            CircuitBreakerOptions = new HttpCircuitBreakerStrategyOptions { SamplingDuration = TimeSpan.FromMinutes(2) },
            AttemptTimeoutOptions = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(1) },
            TotalRequestTimeoutOptions = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(2) }
        };

        services.AddHttpClient<AtomFeedService>("AtomFeedSync", BuildOracleHcmAuthClient)
            .AddStandardResilienceHandler(options => ApplyResilienceOptions(options, commonHttpOptions));

        services.AddHttpClient<IEmployeeSyncService, EmployeeSyncService>("EmployeeSync", BuildOracleHcmAuthClient)
            .AddStandardResilienceHandler(options => ApplyResilienceOptions(options, commonHttpOptions));

        services.AddHttpClient<PayrollSyncClient>("PayrollSync", BuildOracleHcmAuthClient)
            .AddStandardResilienceHandler(options => ApplyResilienceOptions(options, commonHttpOptions));
    }

    private static void ApplyResilienceOptions(HttpStandardResilienceOptions options, HttpResilienceOptions commonOptions)
    {
        options.CircuitBreaker = commonOptions.CircuitBreakerOptions;
        options.AttemptTimeout = commonOptions.AttemptTimeoutOptions;
        options.TotalRequestTimeout = commonOptions.TotalRequestTimeoutOptions;
    }

    private static IHostApplicationBuilder AddOracleHcmMessaging(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumer<OracleHcmMessageConsumer>();
            x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
        });

        return builder;
    }

    private static void BuildOracleHcmAuthClient(IServiceProvider services, HttpClient client)
    {
        var config = services.GetRequiredService<OracleHcmConfig>();
        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}"));

        client.BaseAddress = new Uri(config.BaseAddress, UriKind.Absolute);
        client.DefaultRequestHeaders.Add(FrameworkVersionHeader, config.RestFrameworkVersion);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); // Specify JSON format
    }
}
