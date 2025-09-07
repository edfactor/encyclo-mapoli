using Demoulas.Common.Api.Extensions;
using Demoulas.Common.Contracts.Configuration;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.Common.Logging.Extensions;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Api.Extensions;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.LogMasking;
using Demoulas.ProfitSharing.Data;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Interceptors;
using Demoulas.ProfitSharing.Endpoints.HealthCheck;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Extensions;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Security.Extensions;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.Security.Extensions;
using Demoulas.Util.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSwag.Generation.AspNetCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

if (!builder.Environment.IsTestEnvironment())
{
    builder.Configuration
        .AddJsonFile($"credSettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddUserSecrets<Program>();
}
else
{
    builder.Configuration
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
}

ElasticSearchConfig smartConfig = new ElasticSearchConfig();
builder.Configuration.Bind("Logging:Smart", smartConfig);

FileSystemLogConfig fileSystemLog = new FileSystemLogConfig();
builder.Configuration.Bind("Logging:FileSystem", fileSystemLog);

smartConfig.MaskingOperators = [new UnformattedSocialSecurityNumberMaskingOperator()];
builder.SetDefaultLoggerConfiguration(smartConfig, fileSystemLog);

_ = builder.AddSecurityServices();

if (!builder.Environment.IsTestEnvironment() && Environment.GetEnvironmentVariable("YEMATCH_USE_TEST_CERTS") == null)
{
    builder.Services.AddOktaSecurity(builder.Configuration);
}
else
{
    builder.Services.AddTestingSecurity(builder.Configuration);
}

builder.ConfigureSecurityPolicies();

string[] allowedOrigins = [
        "https://ps.qa.demoulas.net",
        "https://ps.uat.demoulas.net",
        "https://ps.demoulas.net"
];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(pol =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Local development: permissive CORS to simplify local testing
            _ = pol.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .WithExposedHeaders("Location", "x-demographic-data-source");
        }
        else
        {
            // Non-dev: restrict to known UI origins only
            _ = pol.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Location", "x-demographic-data-source");
        }
    });
});

OracleHcmConfig oracleHcmConfig = builder.Configuration.GetSection("OracleHcm").Get<OracleHcmConfig>()
                                  ?? new OracleHcmConfig { BaseAddress = string.Empty, DemographicUrl = string.Empty };

builder.AddOracleHcmSynchronization(oracleHcmConfig);

builder.AddDatabaseServices((services, factoryRequests) =>
{
    // Register contexts without immediately resolving the interceptor
    factoryRequests.Add(ContextFactoryRequest.Initialize<ProfitSharingDbContext>("ProfitSharing",
        interceptorFactory: sp => [
            sp.GetRequiredService<AuditSaveChangesInterceptor>(),
            sp.GetRequiredService<BeneficiarySaveChangesInterceptor>(),
            sp.GetRequiredService<BeneficiaryContactSaveChangesInterceptor>()
        ], denyCommitRoles: [Role.ITDEVOPS, Role.AUDITOR]));
    factoryRequests.Add(ContextFactoryRequest.Initialize<ProfitSharingReadOnlyDbContext>("ProfitSharing"));
    factoryRequests.Add(ContextFactoryRequest.Initialize<DemoulasCommonDataContext>("ProfitSharing"));
});
builder.AddProjectServices();

void OktaSettingsAction(OktaSwaggerConfiguration settings)
{
    builder.Configuration.Bind("Okta", settings);
}

void OktaDocumentSettings(AspNetCoreOpenApiDocumentGeneratorSettings settings)
{
    settings.OperationProcessors.Add(new SwaggerImpersonationHeader());
    settings.OperationProcessors.Add(new SwaggerAuthorizationDetails());
}

builder.ConfigureDefaultEndpoints(meterNames: [],
        activitySourceNames: [OracleHcmActivitySource.Instance.Name])
    .AddSwaggerOpenApi(oktaSettingsAction: OktaSettingsAction, documentSettingsAction: OktaDocumentSettings)
    .AddSwaggerOpenApi(version: 2, oktaSettingsAction: OktaSettingsAction, documentSettingsAction: OktaDocumentSettings);

builder.Services.AddHealthChecks().AddCheck<EnvironmentHealthCheck>("Environment");


builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromMinutes(1);       // Initial delay before the first run
    options.Period = TimeSpan.FromMinutes(10);     // How often health checks are run
    options.Predicate = _ => true;
});

builder.Services.AddSingleton<IHealthCheckPublisher, HealthCheckResultLogger>();
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateOnBuild = true;
});

WebApplication app = builder.Build();

app.UseCors();
app.UseSecurityHeaders();
app.UseDemographicHeaders();
app.UseSensitiveValueMasking();
app.UseDefaultEndpoints(OktaSettingsAction)
    .UseReDoc(settings =>
    {
        settings.Path = "/redoc";
        settings.DocumentPath = "/swagger/Release 1.0/swagger.json"; // Single document
    });

OktaSwaggerConfiguration oktaSwaggerConfiguration = OktaSwaggerConfiguration.Empty();
OktaSettingsAction(oktaSwaggerConfiguration);
app.MapScalarApiReference(options =>
{
    options.OpenApiRoutePattern = "/swagger/Release 1.0/swagger.json";
    options.Theme = ScalarTheme.DeepSpace;
});

await app.RunAsync();

namespace Demoulas.ProfitSharing.Api
{
    public partial class Program
    {
    }
}
