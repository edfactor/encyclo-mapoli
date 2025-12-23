# Part 5c: Telemetry & Middleware

**Estimated Time:** 20 minutes  
**Prerequisites:** [Part 5b Complete](./05b-security-services.md)  
**Next:** [Part 6a: Health Checks](./06a-health-checks.md)

---

## 🎯 Overview

Telemetry infrastructure provides:

- **OpenTelemetry Integration** - Distributed tracing
- **Custom ActivitySource** - Per-endpoint tracking
- **Endpoint Instrumentation Middleware** - Request/response metrics
- **Session Tracking** - User journey correlation

---

## 📊 AddProfitSharingTelemetry Extension

### Extensions/TelemetryExtension.cs

```csharp
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection;

namespace MySolution.Api.Extensions;

public static class TelemetryExtension
{
    public static IServiceCollection AddProfitSharingTelemetry(
        this IServiceCollection services,
        string serviceName)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
                }))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.EnrichWithHttpRequest = (activity, request) =>
                    {
                        activity.SetTag("http.request.size", request.ContentLength);
                    };
                    options.EnrichWithHttpResponse = (activity, response) =>
                    {
                        activity.SetTag("http.response.size", response.ContentLength);
                    };
                })
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation(options =>
                {
                    options.SetDbStatementForText = true;
                    options.SetDbStatementForStoredProcedure = true;
                })
                .AddSource("MySolution.Endpoints"));  // Custom ActivitySource

        return services;
    }
}
```

---

## 🔧 Endpoint Instrumentation Middleware

### Middleware/EndpointInstrumentationMiddleware.cs

```csharp
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MySolution.Api.Middleware;

public class EndpointInstrumentationMiddleware
{
    private static readonly ActivitySource _activitySource = new("MySolution.Endpoints");
    private readonly RequestDelegate _next;
    private readonly ILogger<EndpointInstrumentationMiddleware> _logger;

    public EndpointInstrumentationMiddleware(
        RequestDelegate next,
        ILogger<EndpointInstrumentationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint()?.DisplayName ?? "Unknown";

        using var activity = _activitySource.StartActivity(
            $"{context.Request.Method} {endpoint}",
            ActivityKind.Server);

        if (activity is not null)
        {
            // Extract or create session ID
            var sessionId = context.Request.Headers["X-Session-ID"].FirstOrDefault()
                ?? Guid.NewGuid().ToString();

            activity.SetTag("session.id", sessionId);
            activity.SetTag("endpoint.name", endpoint);
            activity.SetTag("http.method", context.Request.Method);
            activity.SetTag("http.path", context.Request.Path);

            // Add user info if authenticated
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst("sub")?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    activity.SetTag("user.id", userId);
                }
            }

            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["SessionId"] = sessionId,
                ["Endpoint"] = endpoint,
                ["TraceId"] = activity.TraceId.ToString(),
                ["SpanId"] = activity.SpanId.ToString()
            });

            _logger.LogInformation("Request started: {Method} {Path}",
                context.Request.Method, context.Request.Path);

            var sw = Stopwatch.StartNew();
            try
            {
                await _next(context);
                sw.Stop();

                activity.SetTag("http.status_code", context.Response.StatusCode);
                activity.SetTag("duration_ms", sw.ElapsedMilliseconds);

                _logger.LogInformation("Request completed: {Method} {Path} - {StatusCode} in {Duration}ms",
                    context.Request.Method, context.Request.Path,
                    context.Response.StatusCode, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                activity.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity.RecordException(ex);

                _logger.LogError(ex, "Request failed: {Method} {Path} after {Duration}ms",
                    context.Request.Method, context.Request.Path, sw.ElapsedMilliseconds);
                throw;
            }
        }
        else
        {
            await _next(context);
        }
    }
}
```

---

## 🔌 Middleware Registration

### In Program.cs

```csharp
// CRITICAL: Middleware ordering matters!
// See Part 3 for complete ordering diagram

app.UseCors("DevelopmentPolicy");
app.UseNoCacheHeaders();
app.UseDemographicHeaders();
app.UseSensitiveValueMasking();
app.UseDefaultEndpoints();  // Includes UseAuthentication, UseAuthorization

// Endpoint instrumentation MUST be last (wraps entire lifecycle)
app.UseMiddleware<EndpointInstrumentationMiddleware>();

app.UseFastEndpoints();
```

---

## 📈 Custom Metrics (Optional)

### Metrics/EndpointMetrics.cs

```csharp
using System.Diagnostics.Metrics;

namespace MySolution.Api.Metrics;

public static class EndpointMetrics
{
    private static readonly Meter _meter = new("MySolution.Endpoints");

    public static readonly Counter<long> RequestsTotal = _meter.CreateCounter<long>(
        "endpoint.requests.total",
        description: "Total number of endpoint requests");

    public static readonly Histogram<double> RequestDuration = _meter.CreateHistogram<double>(
        "endpoint.request.duration",
        unit: "ms",
        description: "Endpoint request duration in milliseconds");

    public static readonly Counter<long> ErrorsTotal = _meter.CreateCounter<long>(
        "endpoint.errors.total",
        description: "Total number of endpoint errors");
}
```

---

## ✅ Validation Checklist - Part 5c

- [ ] **TelemetryExtension.cs** created
- [ ] **OpenTelemetry** configured with ActivitySource
- [ ] **EndpointInstrumentationMiddleware** implemented
- [ ] **Middleware registered** in correct order (last)
- [ ] **Session tracking** via X-Session-ID header
- [ ] **Custom metrics** defined (optional)

---

## 🎓 Key Takeaways - Part 5c

1. **ActivitySource** - Custom tracing for endpoints
2. **Session Tracking** - Correlate requests across user journey
3. **Middleware Last** - Must wrap entire request lifecycle
4. **Structured Logging** - Use scopes for correlation

---

**Next:** [Part 6a: Health Checks](./06a-health-checks.md)
