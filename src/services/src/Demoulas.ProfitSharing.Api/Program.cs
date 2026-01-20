using Demoulas.Common.Api.Extensions;
using Demoulas.Common.Contracts.Configuration;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Services.Contexts;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.Common.Logging.Extensions;
using Demoulas.ProfitSharing.Api.Extensions;
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
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSwag.AspNetCore;
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
    new SensitiveValueMaskingOperator(builder.Environment)
];

_ = builder.SetDefaultLoggerConfiguration(logConfig);
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
    factoryRequests.Add(ContextFactoryRequest.Initialize<IDemoulasCommonWarehouseContext, DemoulasCommonWarehouseContext>("Warehouse"));
    factoryRequests.Add(ContextFactoryRequest.Initialize<IStoreDbContext, StoreDbContext>("Warehouse"));
});
builder.AddProjectServices();

void OktaSettingsAction(OktaSwaggerConfiguration settings)
{
    builder.Configuration.Bind("Okta", settings);
}

builder.ConfigureDefaultEndpoints(meterNames: [],
        activitySourceNames: [OracleHcmActivitySource.Instance.Name, "Demoulas.ProfitSharing.Endpoints"]
    , useNavigationEndpoints: true, useAuditEventEndpoints: true)
    .AddSwaggerOpenApi(oktaSettingsAction: OktaSettingsAction, impersonationRole: Role.IMPERSONATION,
        swaggerAvailableRoles: new List<string>
        {
            Role.ADMINISTRATOR,
            Role.AUDITOR,
            Role.BENEFICIARY_ADMINISTRATOR,
            Role.DISTRIBUTIONSCLERK,
            Role.EXECUTIVEADMIN,
            Role.FINANCEMANAGER,
            Role.HARDSHIPADMINISTRATOR,
            Role.HR_READONLY,
            Role.ITDEVOPS,
            Role.ITOPERATIONS,
            Role.SSN_UNMASKING
        })
    .AddSwaggerOpenApi(version: 2, oktaSettingsAction: OktaSettingsAction, impersonationRole: Role.IMPERSONATION,
        swaggerAvailableRoles: new List<string>
        {
            Role.ADMINISTRATOR,
            Role.AUDITOR,
            Role.BENEFICIARY_ADMINISTRATOR,
            Role.DISTRIBUTIONSCLERK,
            Role.EXECUTIVEADMIN,
            Role.FINANCEMANAGER,
            Role.HARDSHIPADMINISTRATOR,
            Role.HR_READONLY,
            Role.ITDEVOPS,
            Role.ITOPERATIONS,
            Role.SSN_UNMASKING
        });

// Add Profit Sharing specific telemetry (extends base Aspire setup)
builder.Services.AddProfitSharingTelemetry(builder.Configuration);

builder.Services.AddHealthChecks().AddCheck<EnvironmentHealthCheck>("Environment");

builder.Services.AddEndpointsApiExplorer();
builder.AddProfitSharingRateLimiting();


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

app.Services.TryAddOpenApiSecuritySchemeFixProcessor();

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
app.UseProfitSharingRateLimiting();

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

bool nswagWarmup = Environment.GetEnvironmentVariable("NSWAG_APIEXPLORER_WARMUP") == "1";
bool nswagDump = Environment.GetEnvironmentVariable("NSWAG_APIEXPLORER_DUMP") == "1";

if (nswagWarmup || nswagDump)
{
    try
    {
        // FastEndpoints.Swagger adds a schema validation processor which tries to create scopes using
        // FastEndpoints' internal service resolver. Under the NSwag launcher this can trip an
        // ObjectDisposedException during schema generation. We don't need validation for build-time
        // doc generation, so remove it when NSwag is running.
        var registrationsForNswag = app.Services.GetServices<OpenApiDocumentRegistration>().ToArray();
        foreach (OpenApiDocumentRegistration reg in registrationsForNswag)
        {
            object? generatorSettings =
                reg.GetType().GetProperty("GeneratorSettings")?.GetValue(reg)
                ?? reg.GetType().GetProperty("Settings")?.GetValue(reg)
                ?? reg.GetType().GetProperty("DocumentGeneratorSettings")?.GetValue(reg);

            if (generatorSettings is null)
            {
                continue;
            }

            object? schemaSettings = generatorSettings.GetType().GetProperty("SchemaSettings")?.GetValue(generatorSettings);
            if (schemaSettings is null)
            {
                continue;
            }

            object? schemaProcessorsObj = schemaSettings.GetType().GetProperty("SchemaProcessors")?.GetValue(schemaSettings);
            if (schemaProcessorsObj is not System.Collections.IEnumerable schemaProcessors)
            {
                continue;
            }

            var toRemove = new List<object>();
            foreach (object processor in schemaProcessors)
            {
                if (processor.GetType().FullName == "FastEndpoints.Swagger.ValidationSchemaProcessor")
                {
                    toRemove.Add(processor);
                }
            }

            var removeMethod = schemaProcessorsObj.GetType().GetMethod("Remove");
            if (removeMethod is null)
            {
                continue;
            }

            foreach (object processor in toRemove)
            {
                _ = removeMethod.Invoke(schemaProcessorsObj, [processor]);
            }
        }

        await app.StartAsync();

        if (nswagDump)
        {
            var registrations = app.Services.GetServices<OpenApiDocumentRegistration>().ToArray();
            Console.WriteLine($"NSWAG_APIEXPLORER_DUMP=1: OpenApiDocumentRegistration count: {registrations.Length}");

            foreach (OpenApiDocumentRegistration reg in registrations)
            {
                Console.WriteLine($"  - documentName: '{reg.DocumentName}'");
            }

            var endpointCount = app.Services
                .GetServices<EndpointDataSource>()
                .SelectMany(ds => ds.Endpoints)
                .Count();

            Console.WriteLine($"NSWAG_APIEXPLORER_DUMP=1: EndpointDataSource endpoint count: {endpointCount}");
        }

        IApiDescriptionGroupCollectionProvider? apiExplorer = app.Services.GetService<IApiDescriptionGroupCollectionProvider>();

        if (apiExplorer is not null)
        {
            // Force ApiExplorer to initialize after endpoints are mapped.
            ApiDescriptionGroupCollection groups = apiExplorer.ApiDescriptionGroups;

            if (nswagDump)
            {
                var count = groups.Items.Sum(g => g.Items.Count);
                Console.WriteLine($"NSWAG_APIEXPLORER_DUMP=1: ApiExplorer item count: {count}");
            }

            if (nswagDump)
            {
                Console.WriteLine("NSWAG_APIEXPLORER_DUMP=1: dumping ApiExplorer duplicates...");

                var duplicates = groups.Items
                    .SelectMany(g => g.Items)
                    .Where(d => d.RelativePath is not null && d.HttpMethod is not null)
                    .GroupBy(d => (Method: d.HttpMethod!, Path: "/" + d.RelativePath!))
                    .Where(g => g.Count() > 1)
                    .OrderBy(g => g.Key.Method)
                    .ThenBy(g => g.Key.Path)
                    .ToArray();

                foreach (var dup in duplicates)
                {
                    Console.WriteLine($"NSwag ApiExplorer duplicate: {dup.Key.Method} {dup.Key.Path} (count: {dup.Count()})");

                    foreach (ApiDescription item in dup)
                    {
                        Console.WriteLine($"  - DisplayName: {item.ActionDescriptor.DisplayName}");
                    }
                }

                if (duplicates.Length == 0)
                {
                    Console.WriteLine("NSwag ApiExplorer duplicate dump: none found.");
                }
            }
        }
        else
        {
            Console.WriteLine("NSwag ApiExplorer warmup/dump: IApiDescriptionGroupCollectionProvider not registered.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"NSwag ApiExplorer warmup/dump failed: {ex}");
    }
}

await app.RunAsync();

namespace Demoulas.ProfitSharing.Api
{
    public partial class Program
    {
    }
}
