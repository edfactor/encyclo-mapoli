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

        // Get or create session ID for user journey tracking
        string sessionId = GetOrCreateSessionId(context);
        
        // Store session ID in HttpContext.Items so downstream telemetry can access it
        // (cookies won't be sent to client until response completes, so downstream code
        // can't read from request cookies on the same request)
        context.Items["ps-session-id"] = sessionId;

        if (activity is not null)
        {
            activity.DisplayName = endpointName;
            activity.SetTag("endpoint.name", endpointName);
            activity.SetTag("http.route", context.Request.Path.ToString());
            activity.SetTag("http.method", context.Request.Method);
            activity.SetTag("session.id", sessionId);
        }

        IAppUser? appUser = context.RequestServices.GetService(typeof(IAppUser)) as IAppUser;
        string userEmail = appUser?.Email ?? appUser?.UserName ?? "Unknown";
        string userName = appUser?.UserName ?? "Unknown";

        if (activity is not null)
        {
            // Unique identifier for user journey tracking (email preferred, fallback to username)
            activity.SetTag("user.id", userEmail);
            activity.SetTag("user.name", userName);
            activity.SetTag("user.roles", string.Join(",", appUser?.GetUserAllRoles() ?? new List<string>()));
        }

        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["UserId"] = userEmail,
            ["UserName"] = userName,
            ["SessionId"] = sessionId,
            ["Endpoint"] = endpointName,
        });

        await _next(context);
    }

    /// <summary>
    /// Gets an existing session ID from cookies or creates a new one.
    /// Session ID is used to correlate all requests from a user session for journey tracking.
    /// </summary>
    private static string GetOrCreateSessionId(HttpContext context)
    {
        const string SessionCookieName = "ps-session-id";

        if (context.Request.Cookies.TryGetValue(SessionCookieName, out var existingSessionId) &&
            !string.IsNullOrEmpty(existingSessionId))
        {
            return existingSessionId;
        }

        // Create new session ID (use correlation ID as base for traceability)
        var newSessionId = Guid.NewGuid().ToString("N").Substring(0, 20);

        // Set secure session cookie (HttpOnly to prevent JavaScript access, Secure for HTTPS)
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(8) // Standard 8-hour session timeout
        };

        context.Response.Cookies.Append(SessionCookieName, newSessionId, cookieOptions);
        return newSessionId;
    }
}

public static class EndpointInstrumentationExtensions
{
    public static IApplicationBuilder UseEndpointInstrumentation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<EndpointInstrumentationMiddleware>();
    }
}
