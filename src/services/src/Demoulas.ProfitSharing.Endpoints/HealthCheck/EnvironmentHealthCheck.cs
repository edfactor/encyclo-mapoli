using Demoulas.Common.Api.Utilities;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Demoulas.ProfitSharing.Endpoints.HealthCheck;

using System.Reflection;
using Microsoft.AspNetCore.Hosting;
public class EnvironmentHealthCheck : IHealthCheck
{
    private readonly IWebHostEnvironment _env;
    private readonly AppVersionInfo _appVersion;
    private static readonly DateTime _startupTime = DateTime.UtcNow;

    public EnvironmentHealthCheck(IWebHostEnvironment env, AppVersionInfo appVersion)
    {
        _env = env;
        _appVersion = appVersion;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
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
            { "UtcNow", DateTimeOffset.UtcNow.ToString("o") }
        };

        return Task.FromResult(HealthCheckResult.Healthy("Environment check", data));
    }
}

