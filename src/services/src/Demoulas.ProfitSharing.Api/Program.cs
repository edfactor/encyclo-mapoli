using System.Diagnostics;
using Demoulas.Common.Api.Extensions;
using Demoulas.Common.Contracts.Configuration;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Api.Extensions;
using Demoulas.ProfitSharing.Common.ActivitySources;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.Security;
using Demoulas.Util.Extensions;
using FastEndpoints.Security;
using MassTransit.Monitoring;
using Microsoft.AspNetCore.Authentication;
using NSwag.Generation.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

builder.Configuration
    .AddJsonFile($"credSettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true);


ElasticSearchConfig smartConfig = new ElasticSearchConfig();
builder.Configuration.Bind("Logging:Smart", smartConfig);

await builder.SetDefaultLoggerConfigurationAsync(smartConfig);

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

builder.ConfigurePolicies();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(pol =>
    {
        _ = pol.AllowAnyMethod() // Specify the allowed methods, e.g., GET, POST, etc.
        .AllowAnyHeader()
        .AllowAnyOrigin();
    });
});

List<ContextFactoryRequest> list = new List<ContextFactoryRequest>
{
    ContextFactoryRequest.Initialize<ProfitSharingDbContext>("ProfitSharing"),
    ContextFactoryRequest.Initialize<ProfitSharingReadOnlyDbContext>("ProfitSharing"),
    ContextFactoryRequest.Initialize<DemoulasCommonDataContext>("StoreInfo")
};

#if RUSS
await builder.AddDatabaseServices(list, true, true);
#else
await builder.AddDatabaseServices(list);
#endif

builder.AddCachingServices();
builder.AddProjectServices();


void OktaSettingsAction(OktaSwaggerConfiguration settings)
{
    builder.Configuration.Bind("Okta", settings);
}

void OktaDocumentSettings(AspNetCoreOpenApiDocumentGeneratorSettings settings)
{
    settings.OperationProcessors.Add(new SwaggerImpersonationHeader());
}

builder.ConfigureDefaultEndpoints(meterNames: new[] { InstrumentationOptions.MeterName },
        activitySourceNames: new[] { OracleHcmActivitySource.Instance.Name })
    .AddSwaggerOpenApi(oktaSettingsAction: OktaSettingsAction, documentSettingsAction: OktaDocumentSettings)
    .AddSwaggerOpenApi(version: 2, oktaSettingsAction: OktaSettingsAction, documentSettingsAction: OktaDocumentSettings);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() && Debugger.IsAttached)
{
    // Put code here that ONLY runs when attached to the debugger.
}

app.UseCors();

app.UseDefaultEndpoints(OktaSettingsAction);

await app.RunAsync();


namespace Demoulas.ProfitSharing.Api
{
    public partial class Program
    {
    }
}
