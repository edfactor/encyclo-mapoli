using Demoulas.Common.Api.Utilities;
using Demoulas.Common.Contracts.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Demoulas.ProfitSharing.Endpoints.HealthCheck;

using System.Reflection;
using Microsoft.AspNetCore.Hosting;
public class EnvironmentHealthCheck : IHealthCheck
{
    private readonly IWebHostEnvironment _env;
    private readonly AppVersionInfo _appVersion;
    private readonly IConfiguration _config;
    private static readonly DateTime _startupTime = DateTime.UtcNow;

    public EnvironmentHealthCheck(IWebHostEnvironment env,
        AppVersionInfo appVersion,
        IConfiguration config)
    {
        _env = env;
        _appVersion = appVersion;
        _config = config;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        OktaConfiguration settings = new OktaConfiguration
        {
            OktaDomain = string.Empty,
            AuthorizationServerId = string.Empty,
            Audience = string.Empty,
            RolePrefix = string.Empty
        };
        _config.Bind("Okta", settings);

        var data = new Dictionary<string, object>
        {
            { "Environment", _env.EnvironmentName },
            { "ApplicationName", _env.ApplicationName },
            { "MachineName", Environment.MachineName },
            { "OSVersion", Environment.OSVersion.ToString() },
            { "Framework", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription },
            { "AppVersion", _appVersion.BuildNumber },
            { "CurrentDirectory", Environment.CurrentDirectory },
            { "Uptime", (DateTime.UtcNow - _startupTime).ToString(@"dd\.hh\:mm\:ss") },
            { "UtcNow", DateTimeOffset.UtcNow.ToString("o") },
            { "OktaEnvironmentName", settings.EnvironmentName ?? string.Empty },
            { "OktaRolePrefix", settings.RolePrefix },
            { "OktaAuthorizationServerId", settings.AuthorizationServerId },
            { "OktaAudience", settings.Audience }
        };

        return Task.FromResult(HealthCheckResult.Healthy("Environment check", data));
    }
}

