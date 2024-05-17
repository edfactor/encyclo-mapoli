using System.Reflection;
using Demoulas.ProfitSharing.Api.Utilities;

namespace Demoulas.ProfitSharing.Api.Middleware;

public class VersionMiddleware
{
    private string? _version;
    private readonly RequestDelegate _next;
    private readonly AppVersionInfo _appVersionInfo;

    public VersionMiddleware(RequestDelegate next, AppVersionInfo appVersionInfo)
    {
        CompileVersion();
        _next = next;
        _appVersionInfo = appVersionInfo;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append("X-Api-Version", $"{_appVersionInfo.BuildNumber}-{_appVersionInfo.ShortGitHash}");
            return Task.CompletedTask;
        });

        await _next(context);
    }

    private void CompileVersion()
    {
        var attribute = Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;
        _version ??= attribute?.InformationalVersion;
    }
}
