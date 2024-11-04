using System.Net.Http.Headers;
using System.Text;
using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.OracleHcm.HostedServices;
using Demoulas.ProfitSharing.OracleHcm.Jobs;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Demoulas.ProfitSharing.OracleHcm.Validators;
using Demoulas.Util.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Spi;
using Quartz;

namespace Demoulas.ProfitSharing.OracleHcm.Extensions;
public static class OracleHcmExtension
{
    private const string FrameworkVersionHeader = "REST-Framework-Version";


    public static IHostApplicationBuilder ConfigureOracleHcm(this IHostApplicationBuilder builder)
    {
        OracleHcmConfig oracleHcmConfig = builder.Configuration.GetSection("OracleHcm").Get<OracleHcmConfig>() ??
                                          new OracleHcmConfig { BaseAddress = string.Empty, Url = string.Empty };
        _ = builder.Services.AddSingleton(oracleHcmConfig);

        _ = builder.Services.AddSingleton<OracleEmployeeValidator>();
        _ = builder.Services.AddSingleton<EmployeeSyncJob>();
        _ = builder.Services.AddSingleton<IJobFactory, OracleHcmJobFactory>();
        _ = builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();


        _ = builder.Services.AddHttpClient<IEmployeeSyncService, EmployeeSyncService>((services, client) =>
        {
            OracleHcmConfig config = services.GetRequiredService<OracleHcmConfig>();

            byte[] bytes = Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}");
            string encodedAuth = Convert.ToBase64String(bytes);
            if (!string.IsNullOrEmpty(config.Url))
            {
                client.BaseAddress = new Uri(string.Concat(config.BaseAddress, config.Url), UriKind.Absolute);
            }

            client.DefaultRequestHeaders.Add(FrameworkVersionHeader, config.RestFrameworkVersion);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuth);
        }).AddStandardResilienceHandler(options =>
        {
            options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions { SamplingDuration = TimeSpan.FromMinutes(2) };
            options.AttemptTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(1) };
            options.TotalRequestTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(2) };
        });


        _ = builder.Services.AddHttpClient<PayrollSyncClient>((services, client) =>
        {
            OracleHcmConfig config = services.GetRequiredService<OracleHcmConfig>();

            byte[] bytes = Encoding.UTF8.GetBytes($"{config.PayrollUsername}:{config.PayrollPassword}");
            string encodedAuth = Convert.ToBase64String(bytes);
            if (!string.IsNullOrEmpty(config.Url))
            {
                client.BaseAddress = new Uri(string.Concat(config.BaseAddress, config.PayrollUrl), UriKind.Absolute);
            }

            client.DefaultRequestHeaders.Add(FrameworkVersionHeader, config.RestFrameworkVersion);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuth);
        }).AddStandardResilienceHandler(options =>
        {
            options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions { SamplingDuration = TimeSpan.FromMinutes(2) };
            options.AttemptTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(1) };
            options.TotalRequestTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(2) };
        });





        if (!builder.Environment.IsTestEnvironment())
        {
            _ = builder.Services.AddHostedService<OracleHcmHostedService>();
        }


        return builder;
    }
}
