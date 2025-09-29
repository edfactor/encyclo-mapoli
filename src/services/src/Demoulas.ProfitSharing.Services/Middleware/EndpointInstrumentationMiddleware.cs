using System.Diagnostics;
using Demoulas.Common.Contracts.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Middleware;

public sealed class EndpointInstrumentationMiddleware
{
    private static readonly ActivitySource ActivitySource = new("Demoulas.ProfitSharing.Endpoints");
    private readonly RequestDelegate _next;
    private readonly ILogger<EndpointInstrumentationMiddleware> _logger;

    public EndpointInstrumentationMiddleware(RequestDelegate next, ILogger<EndpointInstrumentationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string endpointName = context.GetEndpoint()?.DisplayName ?? context.Request.Path;

        using Activity? activity = Activity.Current is null
            ? ActivitySource.StartActivity(endpointName, ActivityKind.Server)
            : null;

        if (activity is not null)
        {
            activity.DisplayName = endpointName;
            activity.SetTag("endpoint.name", endpointName);
            activity.SetTag("http.route", context.Request.Path.ToString());
            activity.SetTag("http.method", context.Request.Method);
        }

        IAppUser? appUser = context.RequestServices.GetService(typeof(IAppUser)) as IAppUser;
        string userName = appUser?.UserName ?? "Unknown";
        activity?.SetTag("enduser.id", userName);

        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["UserName"] = userName,
            ["Endpoint"] = endpointName,
        });

        await _next(context);
    }
}

public static class EndpointInstrumentationExtensions
{
    public static IApplicationBuilder UseEndpointInstrumentation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<EndpointInstrumentationMiddleware>();
    }
}
