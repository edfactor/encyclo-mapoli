using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Api.Extensions;
using Demoulas.ProfitSharing.Common.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Configuration;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.ServiceDefaults;
using Demoulas.StoreInfo.Entities.Contexts;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

builder.Configuration.AddUserSecrets<Program>();

ElasticSearchConfig smartConfig = new ElasticSearchConfig();
builder.Configuration.Bind("Logging:Smart", smartConfig);

builder.SetDefaultLoggerConfiguration(smartConfig);

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
    ContextFactoryRequest.Initialize<StoreInfoDbContext>("StoreInfo")
};

#if RUSS
await builder.AddDatabaseServices(list, true, true);
#else
await builder.AddDatabaseServices(list);
#endif

builder.AddCachingServices();


void OktaSettingsAction(OktaSettings settings)
{
    const string notSet = "Not Set";
    settings = builder.Configuration.GetSection("Okta").Get<OktaSettings>() ?? new OktaSettings { AuthorizationEndpoint = notSet, ClientId = notSet, Issuer = notSet, TokenEndpoint = notSet };
}

builder.ConfigureDefaultEndpoints()
    .AddSwaggerOpenApi(enableJwtBearerAuth: false, oktaSettingsAction: OktaSettingsAction)
    .AddSwaggerOpenApi(version: 2, enableJwtBearerAuth: false, oktaSettingsAction: OktaSettingsAction);

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    if (Debugger.IsAttached)
    {
        // Put code here that ONLY runs when attached to the debugger.
    }
}

app.UseCors();

app.UseDefaultEndpoints();

app.Run();


namespace Demoulas.ProfitSharing.Api
{
    public partial class Program
    {
    }
}
