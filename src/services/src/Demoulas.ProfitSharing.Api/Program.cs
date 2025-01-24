using Demoulas.Common.Api.Extensions;
using Demoulas.Common.Contracts.Configuration;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.Common.Logging.Extensions;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Api.Extensions;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Extensions;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.Security;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Authentication;
using NSwag.Generation.AspNetCore;

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

await builder.SetDefaultLoggerConfigurationAsync(smartConfig, fileSystemLog);

_ = builder.Services.AddTransient<IClaimsTransformation, ImpersonationAndEnvironmentAwareClaimsTransformation>();

var rolePermissionService = new RolePermissionService();
if (!builder.Environment.IsTestEnvironment())
{
    builder.Services.AddOktaSecurity(builder.Configuration, rolePermissionService);
}
else
{
    builder.Services.AddTestingSecurity(builder.Configuration, rolePermissionService);
}

builder.ConfigureSecurityPolicies();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(pol =>
    {
        _ = pol.AllowAnyMethod() // Specify the allowed methods, e.g., GET, POST, etc.
            .AllowAnyHeader()
            .AllowAnyOrigin()
            .WithExposedHeaders("Location");
    });
});

List<ContextFactoryRequest> list =
[
    ContextFactoryRequest.Initialize<ProfitSharingDbContext>("ProfitSharing"),
    ContextFactoryRequest.Initialize<ProfitSharingReadOnlyDbContext>("ProfitSharing"),
    ContextFactoryRequest.Initialize<DemoulasCommonDataContext>("StoreInfo")
];

builder.AddDatabaseServices(list);
builder.AddProjectServices();

OracleHcmConfig oracleHcmConfig = builder.Configuration.GetSection("OracleHcm").Get<OracleHcmConfig>()
                                  ?? new OracleHcmConfig { BaseAddress = string.Empty, DemographicUrl = string.Empty };

builder.AddOracleHcmSynchronization(oracleHcmConfig);


void OktaSettingsAction(OktaSwaggerConfiguration settings)
{
    builder.Configuration.Bind("Okta", settings);
}

void OktaDocumentSettings(AspNetCoreOpenApiDocumentGeneratorSettings settings)
{
    settings.OperationProcessors.Add(new SwaggerImpersonationHeader());
}

builder.ConfigureDefaultEndpoints(meterNames: [],
        activitySourceNames: [OracleHcmActivitySource.Instance.Name])
    .AddSwaggerOpenApi(oktaSettingsAction: OktaSettingsAction, documentSettingsAction: OktaDocumentSettings)
    .AddSwaggerOpenApi(version: 2, oktaSettingsAction: OktaSettingsAction, documentSettingsAction: OktaDocumentSettings);

WebApplication app = builder.Build();

app.UseCors();

app.UseDefaultEndpoints(OktaSettingsAction)
    .UseReDoc(settings =>
    {
        settings.Path = "/redoc";
        settings.DocumentPath = "/swagger/Release 1.0/swagger.json"; // Single document
    });

await app.RunAsync();

namespace Demoulas.ProfitSharing.Api
{
    public partial class Program
    {
    }
}
