using Demoulas.Common.Api.Extensions;
using Demoulas.Common.Contracts.Configuration;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.Common.Logging.Extensions;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Common.Metrics;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Interceptors;
using Demoulas.ProfitSharing.Endpoints.HealthCheck;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Security.Extensions;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.ProfitSharing.Services.LogMasking; // retains AddProjectServices & other extension methods
using Demoulas.ProfitSharing.Services.Middleware;
using Demoulas.ProfitSharing.Services.Serialization;
using Demoulas.Util.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSwag.Generation.AspNetCore;
using QuestPDF;
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

// Configure QuestPDF license for development and production environments
// For development and non-commercial use: Community license (free)
// For commercial use with revenue > $1M USD: requires Commercial license
// See: https://www.questpdf.com/license/
Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

_ = builder.AddSecurityServices();


string[] allowedOrigins = [
        "https://ps.qa.demoulas.net",
        "https://ps.uat.demoulas.net",
        "https://ps.demoulas.net"
];

string[] developmentOrigins = [
        "http://localhost:3100",
        "http://127.0.0.1:3100",
        "https://localhost:3100",
        "https://127.0.0.1:3100"
];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(pol =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Local development: restrict to localhost origins only (PS-2025)
            // Prevents MITM attacks on shared networks (e.g., coffee shop WiFi)
            _ = pol.WithOrigins(developmentOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
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

// Configure JSON serialization with source generation for better performance
builder.Services.ConfigureHttpJsonOptions(o =>
{
    // Insert masking converter at index 0 to ensure highest precedence
    // Pass the host environment to the converter so it can check IsTestEnvironment()
    o.SerializerOptions.Converters.Insert(0, new MaskingJsonConverterFactory(builder.Environment));

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
        ], denyCommitRoles: [Role.ITDEVOPS, Role.AUDITOR, Role.HR_READONLY, Role.SSN_UNMASKING]));
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

// Add no-cache headers to all responses to prevent browser caching of sensitive data
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate, max-age=0";
        context.Response.Headers.Pragma = "no-cache";
        context.Response.Headers.Expires = "0";
        return Task.CompletedTask;
    });
    await next();
});

app.UseDemographicHeaders();

app.UseSensitiveValueMasking();

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
