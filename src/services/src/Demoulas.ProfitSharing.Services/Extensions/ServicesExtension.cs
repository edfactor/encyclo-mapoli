using Demoulas.Common.Caching.Interfaces;
using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.OracleHcm;
using Demoulas.ProfitSharing.Services.HostedServices;
using Demoulas.ProfitSharing.Services.InternalEntities;
using Demoulas.ProfitSharing.Services.Jobs;
using Demoulas.ProfitSharing.Services.Mappers;
using Demoulas.ProfitSharing.Services.Reports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Spi;
using Quartz;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Http.Resilience;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Services.Validators;

namespace Demoulas.ProfitSharing.Services.Extensions;

/// <summary>
/// Provides helper methods for configuring project builder.Services.
/// </summary>
public static class ServicesExtension
{
    public static IHostApplicationBuilder AddProjectServices(this IHostApplicationBuilder builder)
    {
        _ = builder.Services.AddSingleton<IEmployeeSyncJob, EmployeeSyncJob>();
        _ = builder.Services.AddSingleton<OracleEmployeeValidator>();

        _ = builder.Services.AddScoped<IPayClassificationService, PayClassificationService>();
        _ = builder.Services.AddScoped<IDemographicsService, DemographicsService>();
        _ = builder.Services.AddScoped<IYearEndService, YearEndService>();
        _ = builder.Services.AddScoped<IPayProfitService, PayProfitService>();
        _ = builder.Services.AddScoped<ISynchronizationService, SynchronizationService>();


        OracleHcmConfig oktaSettings = builder.Configuration.GetSection("OracleHcm").Get<OracleHcmConfig>() ?? new OracleHcmConfig { Url = string.Empty };
        _ = builder.Services.AddSingleton(oktaSettings);

        _ = builder.Services.AddSingleton<IJobFactory, SimpleJobFactory>();
        _ = builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        _ = builder.Services.AddSingleton<IDemographicsServiceInternal, DemographicsService>();
        _ = builder.Services.AddSingleton<ISynchronizationService, SynchronizationService>();
        _ = builder.Services.AddSingleton<IStoreService, StoreService>();

        _ = builder.Services.AddHttpClient<OracleDemographicsService>((services, client) =>
        {
            OracleHcmConfig config = services.GetRequiredService<OracleHcmConfig>();

            byte[] bytes = Encoding.UTF8.GetBytes($"{config.Username}:{config.Password}");
            string encodedAuth = Convert.ToBase64String(bytes);

            client.BaseAddress = new Uri(config.Url, UriKind.Absolute);
            client.DefaultRequestHeaders.Add("REST-Framework-Version", config.RestFrameworkVersion);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuth);
        }).AddStandardResilienceHandler(options =>
        {
            options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions { SamplingDuration = TimeSpan.FromMinutes(2) };
            options.AttemptTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(1) };
            options.TotalRequestTimeout = new HttpTimeoutStrategyOptions { Timeout = TimeSpan.FromMinutes(2) };
        });



        _ = builder.Services.AddSingleton<IBaseCacheService<PayClassificationResponseCache>, PayClassificationHostedService>();


        builder.ConfigureMassTransitServices();

        #region Mappers

        builder.Services.AddSingleton<AddressMapper>();
        builder.Services.AddSingleton<ContactInfoMapper>();
        builder.Services.AddSingleton<DemographicMapper>();
        builder.Services.AddSingleton<TerminationCodeMapper>();
        builder.Services.AddSingleton<PayFrequencyMapper>();
        builder.Services.AddSingleton<GenderMapper>();
        builder.Services.AddSingleton<EmploymentTypeMapper>();
        builder.Services.AddSingleton<DepartmentMapper>();
        builder.Services.AddSingleton<PayProfitMapper>();
        builder.Services.AddSingleton<ZeroContributionReasonMapper>();
        builder.Services.AddSingleton<BeneficiaryTypeMapper>();
        builder.Services.AddSingleton<EmployeeTypeMapper>();

        #endregion

        return builder;
    }
}
