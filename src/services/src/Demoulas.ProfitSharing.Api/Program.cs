using System.Diagnostics;
using Demoulas.ProfitSharing.Api.Extensions;
using Demoulas.ProfitSharing.ServiceDefaults;

var builder = WebApplication.CreateBuilder();

builder.Configuration.AddUserSecrets<Program>();

#if !DEBUG
if (!Debugger.IsAttached)
{
    // This is ONLY used when deployed.
    string configFile = $"credSettings.{builder.Environment.EnvironmentName}.json";
    builder.Configuration.AddJsonFile(configFile);
}
#endif

builder.SetDefaultLoggerConfiguration("Demoulas.ProfitSharing");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(pol =>
    {
        _ = pol.AllowAnyMethod() // Specify the allowed methods, e.g., GET, POST, etc.
            .AllowAnyHeader()
            .AllowAnyOrigin();
    });
});

//builder.AddDatabaseServices("AccountsReceivable");
builder.AddCachingServices();

builder.ConfigureDefaultEndpoints();


var app = builder.Build();

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