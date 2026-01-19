using System.Runtime.InteropServices;
using Demoulas.Common.Api.Utilities;
using Demoulas.Common.Contracts.Configuration;
using Demoulas.ProfitSharing.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Demoulas.ProfitSharing.Endpoints.HealthCheck;

public class EnvironmentHealthCheck : IHealthCheck
{
    private static readonly DateTime _startupTime = DateTime.UtcNow;
    private readonly AppVersionInfo _appVersion;
    private readonly OktaConfiguration _config;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IReportRunnerService _reportRunnerService;

    public EnvironmentHealthCheck(IWebHostEnvironment env,
        AppVersionInfo appVersion,
        OktaConfiguration config,
        IHttpContextAccessor httpContextAccessor,
        IReportRunnerService reportRunnerService)
    {
        _env = env;
        _appVersion = appVersion;
        _config = config;
        _httpContextAccessor = httpContextAccessor;
        _reportRunnerService = reportRunnerService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        Dictionary<string, object> data = new()
        {
            { "Environment", _env.EnvironmentName },
            { "ApplicationName", _env.ApplicationName },
            { "MachineName", Environment.MachineName },
            { "WorkingSet", Environment.WorkingSet },
            { "OSVersion", Environment.OSVersion.ToString() },
            { "Framework", RuntimeInformation.FrameworkDescription },
            { "AppVersion", _appVersion.BuildNumber },
            { "CurrentDirectory", Environment.CurrentDirectory },
            { "Uptime", (DateTime.UtcNow - _startupTime).ToString(@"dd\.hh\:mm\:ss") },
            { "UtcNow", DateTimeOffset.UtcNow.ToString("o") },
            { "OktaEnvironmentName", _config.EnvironmentName ?? string.Empty },
            { "OktaRolePrefix", _config.RolePrefix }
        };

        string? reportSelector = _httpContextAccessor.HttpContext?.Request.Query["reportSelector"].ToString();
        if (!string.IsNullOrWhiteSpace(reportSelector))
        {
            data.Add("ReportRunner", await _reportRunnerService.IncludeReportInformation(reportSelector, cancellationToken));
        }

        return HealthCheckResult.Healthy("Environment check", data);
    }
}
