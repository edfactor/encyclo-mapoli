using Demoulas.Common.Api.Extensions;
using Demoulas.Common.Contracts.Configuration;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.Common.Logging.Extensions;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Api.Extensions;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Metrics;
using Demoulas.ProfitSharing.Common.Telemetry;
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
using Demoulas.ProfitSharing.Services.LogMasking; // retains AddProjectServices & other extension methods
using Demoulas.ProfitSharing.Services.Middleware;
using Demoulas.ProfitSharing.Services.Serialization;
using Demoulas.Security.Extensions;
using Demoulas.Util.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSwag.Generation.AspNetCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

// Note: Kestrel configuration only applies to dev environment
// Production uses IIS with its own timeout configuration in web.config
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        // 3-minute timeout for dev (provides buffer for 2-minute endpoint timeout)
        options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(3);
        options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
    });
}

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

// Configure logging - configuration read from SmartLogging section in appsettings
LoggingConfig logConfig = new();
builder.Configuration.Bind("SmartLogging", logConfig);

logConfig.MaskingOperators = [
    new UnformattedSocialSecurityNumberMaskingOperator(),
    new SensitiveValueMaskingOperator()
];

_ = builder.SetDefaultLoggerConfiguration(logConfig);

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

// Configure JSON serialization with source generation for better performance
builder.Services.ConfigureHttpJsonOptions(o =>
{
    // Insert masking converter at index 0 to ensure highest precedence
    o.SerializerOptions.Converters.Insert(0, new MaskingJsonConverterFactory());

    // Add source-generated JSON serializer context for compile-time serialization
    // This reduces reflection overhead and improves startup time
    o.SerializerOptions.TypeInfoResolverChain.Insert(0, Demoulas.ProfitSharing.Api.Serialization.ProfitSharingJsonSerializerContext.Default);
});

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
    activitySourceNames: [OracleHcmActivitySource.Instance.Name, "Demoulas.ProfitSharing.Endpoints"])
    .AddSwaggerOpenApi(oktaSettingsAction: OktaSettingsAction, documentSettingsAction: OktaDocumentSettings)
    .AddSwaggerOpenApi(version: 2, oktaSettingsAction: OktaSettingsAction, documentSettingsAction: OktaDocumentSettings);

// Add Profit Sharing specific telemetry (extends base Aspire setup)
builder.Services.AddProfitSharingTelemetry(builder.Configuration);

builder.Services.AddHealthChecks().AddCheck<EnvironmentHealthCheck>("Environment");


builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromMinutes(1);       // Initial delay before the first run
    options.Period = TimeSpan.FromMinutes(15);     // How often health checks are run
    options.Predicate = _ => true;
});

builder.Services.AddSingleton<IHealthCheckPublisher, HealthCheckResultLogger>();
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateOnBuild = true;
});

WebApplication app = builder.Build();

// Initialize metrics (resolve version) then register gauges
GlobalMeter.InitializeFromServices(app.Services);
GlobalMeter.RegisterObservableGauges();
GlobalMeter.RecordDeploymentStartup();

app.UseCors();
app.UseDemographicHeaders();

app.UseSensitiveValueMasking();

if (app.Environment.IsProduction())
{
    // Breaks swagger, but swagger isn't available in production/UAT anyway
    app.UseSecurityHeaders();
}

app.UseDefaultEndpoints(OktaSettingsAction)
    .UseReDoc(settings =>
    {
        settings.Path = "/redoc";
        settings.DocumentPath = "/swagger/Release 1.0/swagger.json"; // Single document
    });

// Global per-request instrumentation: Activity + log scope
app.UseEndpointInstrumentation();

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
