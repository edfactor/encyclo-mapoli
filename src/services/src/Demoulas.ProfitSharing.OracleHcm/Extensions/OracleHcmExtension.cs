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
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Caching.Extensions;
using Demoulas.ProfitSharing.Services.Mappers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Demoulas.ProfitSharing.OracleHcm.Extensions;

public static class OracleHcmExtension
{
    private const string FrameworkVersionHeader = "REST-Framework-Version";

    public static IHostApplicationBuilder AddOracleHcmBackgroundProcess(this IHostApplicationBuilder builder)
    {
        _ = builder.AddOracleHcmSynchronization();
        _ = builder.Services.AddHostedService<OracleHcmHostedService>();

        return builder;
    }

    public static IHostApplicationBuilder AddOracleHcmSynchronization(this IHostApplicationBuilder builder)
    {
        OracleHcmConfig oracleHcmConfig = builder.Configuration.GetSection("OracleHcm").Get<OracleHcmConfig>() ??
                                          new OracleHcmConfig { BaseAddress = string.Empty, DemographicUrl = string.Empty };

        _ = builder.Services.AddSingleton(oracleHcmConfig);

        // Add Atom feed-based services
        _ = builder.Services.AddSingleton<OracleEmployeeValidator>();
        _ = builder.Services.AddSingleton<EmployeeFullSyncJob>();
        _ = builder.Services.AddSingleton<EmployeeDeltaSyncJob>();
        _ = builder.Services.AddSingleton<PayrollSyncJob>();
        _ = builder.Services.AddSingleton<DemographicsService>();
        _ = builder.Services.AddSingleton<EmployeeMapper>();
        _ = builder.Services.AddSingleton<SyncJobService>();

        _ = builder.Services.AddSingleton<DemographicMapper>();
        _ = builder.Services.AddSingleton<AddressMapper>();
        _ = builder.Services.AddSingleton<ContactInfoMapper>();

        _ = builder.Services.AddSingleton<IDemographicsServiceInternal, DemographicsService>();
        _ = builder.Services.AddSingleton<IJobFactory, OracleHcmJobFactory>();
        _ = builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();


        // Add HttpClient with authorization for Atom feeds
        _ = builder.Services.AddHttpClient<AtomFeedService>("AtomFeedSync", BuildOracleHcmAuthClient).AddStandardResilienceHandler(
            options =>
            {
                options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions { SamplingDuration = TimeSpan.FromMinutes(2) };
                options.AttemptTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(1) };
                options.TotalRequestTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(2) };
            });

        _ = builder.Services.AddHttpClient<IEmployeeSyncService, EmployeeSyncService>("EmployeeSync", BuildOracleHcmAuthClient).AddStandardResilienceHandler(
            options =>
            {
                options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions { SamplingDuration = TimeSpan.FromMinutes(2) };
                options.AttemptTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(1) };
                options.TotalRequestTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(2) };
            });


        _ = builder.Services.AddHttpClient<PayrollSyncClient>("PayrollSync", BuildOracleHcmAuthClient).AddStandardResilienceHandler(options =>
        {
            options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions { SamplingDuration = TimeSpan.FromMinutes(2) };
            options.AttemptTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(1) };
            options.TotalRequestTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(2) };
        });

        _ = builder.Services.AddHealthChecks().AddCheck<OracleHcmHealthCheck>("OracleHcm");

        _ = builder.AddProjectCachingServices();

        _ = builder.AddOracleHcmMessaging();


        return builder;
    }

    private static IHostApplicationBuilder AddOracleHcmMessaging(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumer<OracleHcmMessageConsumer>();

            x.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
        });


        return builder;
    }


    private static void BuildOracleHcmAuthClient(IServiceProvider services, HttpClient client)
    {
        OracleHcmConfig config = services.GetRequiredService<OracleHcmConfig>();

        byte[] bytes = Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}");
        string encodedAuth = Convert.ToBase64String(bytes);
        client.BaseAddress = new Uri(config.BaseAddress, UriKind.Absolute);
        client.DefaultRequestHeaders.Add(FrameworkVersionHeader, config.RestFrameworkVersion);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuth);

        // Specify JSON format
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
