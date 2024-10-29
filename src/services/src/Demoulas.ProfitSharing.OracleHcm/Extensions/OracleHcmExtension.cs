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
    public static IHostApplicationBuilder ConfigureOracleHcm(this IHostApplicationBuilder builder)
    {
        _ = builder.Services.AddHttpClient<IEmployeeSyncService, EmployeeSyncService>((services, client) =>
        {
            OracleHcmConfig config = services.GetRequiredService<OracleHcmConfig>();

            byte[] bytes = Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}");
            string encodedAuth = Convert.ToBase64String(bytes);
            if (!string.IsNullOrEmpty(config.Url))
            {
                client.BaseAddress = new Uri(config.Url, UriKind.Absolute);
            }

            client.DefaultRequestHeaders.Add("REST-Framework-Version", config.RestFrameworkVersion);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuth);
        }).AddStandardResilienceHandler(options =>
        {
            options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions { SamplingDuration = TimeSpan.FromMinutes(2) };
            options.AttemptTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(1) };
            options.TotalRequestTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(2) };
        });

        OracleHcmConfig oracleHcmConfig = builder.Configuration.GetSection("OracleHcm").Get<OracleHcmConfig>() ?? new OracleHcmConfig { Url = string.Empty };
        _ = builder.Services.AddSingleton(oracleHcmConfig);

        _ = builder.Services.AddSingleton<OracleEmployeeValidator>();
        _ = builder.Services.AddSingleton<EmployeeSyncJob>();
        _ = builder.Services.AddSingleton<IJobFactory, OracleHcmJobFactory>();
        _ = builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();


        if (!builder.Environment.IsTestEnvironment())
        {
            _ = builder.Services.AddHostedService<OracleHcmHostedService>();
        }


        return builder;
    }
}
