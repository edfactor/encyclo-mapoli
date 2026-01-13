using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Common.Telemetry;

/// <summary>
/// Middleware to capture telemetry data for all HTTP requests
/// </summary>
public class TelemetryMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TelemetryMiddleware> _logger;
    private readonly TelemetryConfiguration _config;

    private static readonly string[] SensitiveEndpoints = {
        "/demographics", "/member-details", "/beneficiaries"
    };

    public TelemetryMiddleware(RequestDelegate next, ILogger<TelemetryMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _config = new TelemetryConfiguration();
        configuration.GetSection(TelemetryConfiguration.SectionName).Bind(_config);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_config.Enabled)
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var correlationId = Guid.NewGuid().ToString("N")[..8];

        // Start activity for distributed tracing
        using var activity = EndpointTelemetry.ActivitySource.StartActivity("http_request");
        activity?.SetTag("http.method", context.Request.Method);
        activity?.SetTag("http.route", GetEndpointCategory(context.Request.Path));
        activity?.SetTag("correlation_id", correlationId);

        // Extract user context (safely)
        var userRole = GetUserRole(context);
        var userId = GetMaskedUserId(context);

        activity?.SetTag("user.role", userRole);
        if (!string.IsNullOrEmpty(userId))
        {
            activity?.SetTag("user.id_masked", userId);
        }

        Exception? exception = null;
        var originalBodyStream = context.Response.Body;

        try
        {
            // Only capture response body if we need to measure it
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            // Copy response back to original stream first
            context.Response.Body = originalBodyStream;
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);

            // Then measure and record metrics
            var responseSize = responseBody.Length;
            RecordMetrics(context, stopwatch.Elapsed, responseSize, userRole, correlationId);

            // Log large responses
            if (responseSize > _config.LargeResponseThresholdBytes)
            {
                LogLargeResponse(context, responseSize, correlationId, userRole);
            }
        }
        catch (Exception ex)
        {
            exception = ex;
            RecordError(context, ex, userRole, correlationId);
            throw;
        }
        finally
        {
            // Ensure original stream is restored
            context.Response.Body = originalBodyStream;
            stopwatch.Stop();

            activity?.SetTag("http.status_code", context.Response.StatusCode);
            activity?.SetTag("duration_ms", stopwatch.ElapsedMilliseconds);

            if (exception != null)
            {
                activity?.SetTag("error", true);
                activity?.SetTag("exception.type", exception.GetType().Name);
            }
        }
    }

    private void RecordMetrics(HttpContext context, TimeSpan duration, long responseSize, string userRole, string correlationId)
    {
        var endpointCategory = GetEndpointCategory(context.Request.Path);
        var statusCode = context.Response.StatusCode.ToString();

        var tags = new KeyValuePair<string, object?>[]
        {
            new("endpoint_category", endpointCategory),
            new("method", context.Request.Method),
            new("status_code", statusCode),
            new("user_role", userRole)
        };

        // Record request count and duration using existing GlobalMeter
        GlobalMeter.ApiRequests.Add(1, tags);
        GlobalMeter.ApiRequestDurationMs.Record(duration.TotalMilliseconds, tags);
        EndpointTelemetry.ResponseSizeBytes.Record(responseSize, tags);

        // Record large response counter
        if (responseSize > _config.LargeResponseThresholdBytes)
        {
            EndpointTelemetry.LargeResponsesTotal.Add(1,
                new("endpoint_category", endpointCategory),
                new("user_role", userRole));
        }

        // Record sensitive field access if enabled and applicable
        if (_config.EnableSensitiveFieldTracking && IsSensitiveEndpoint(context.Request.Path))
        {
            RecordSensitiveFieldAccess(endpointCategory, correlationId, userRole);
        }
    }

    private void RecordError(HttpContext context, Exception exception, string userRole, string correlationId)
    {
        var endpointCategory = GetEndpointCategory(context.Request.Path);

        EndpointTelemetry.EndpointErrorsTotal.Add(1,
            new("endpoint_category", endpointCategory),
            new("error_type", exception.GetType().Name),
            new("user_role", userRole));

        _logger.LogError(exception,
            "Request failed for endpoint {EndpointCategory} with correlation {CorrelationId} for user role {UserRole}",
            endpointCategory, correlationId, userRole);
    }

    private void RecordSensitiveFieldAccess(string endpointCategory, string correlationId, string userRole)
    {
        EndpointTelemetry.SensitiveFieldAccessTotal.Add(1,
            new("field", "demographics"), // Generic category
            new("endpoint_category", endpointCategory));

        // Log structured event for audit trail (avoid PII in logs)
        _logger.LogInformation(
            "Sensitive field access recorded for endpoint {EndpointCategory} with correlation {CorrelationId} for user role {UserRole}",
            endpointCategory, correlationId, userRole);
    }

    private void LogLargeResponse(HttpContext context, long responseSize, string correlationId, string userRole)
    {
        _logger.LogInformation(
            "Large response detected: {ResponseSizeBytes} bytes for endpoint {EndpointCategory} with correlation {CorrelationId} for user role {UserRole}",
            responseSize, GetEndpointCategory(context.Request.Path), correlationId, userRole);
    }

    private static string GetEndpointCategory(string path)
    {
        // Normalize paths to low-cardinality categories
        var normalizedPath = path.ToLowerInvariant();

        if (normalizedPath.Contains("/demographics")) { return "demographics"; }
        if (normalizedPath.Contains("/beneficiaries")) { return "beneficiaries"; }
        if (normalizedPath.Contains("/reports")) { return "reports"; }
        if (normalizedPath.Contains("/lookups") || normalizedPath.Contains("/tax-codes") ||
            normalizedPath.Contains("/comment-types")) { return "lookups"; }
        if (normalizedPath.Contains("/master-inquiry")) { return "master-inquiry"; }
        if (normalizedPath.Contains("/health")) { return "health"; }

        return "other";
    }

    private static bool IsSensitiveEndpoint(string path)
    {
        var normalizedPath = path.ToLowerInvariant();
        return SensitiveEndpoints.Any(endpoint => normalizedPath.Contains(endpoint));
    }

    private static string GetUserRole(HttpContext context)
    {
        try
        {
            // Extract user role from claims (avoid PII)
            var user = context.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                return user.FindFirst("role")?.Value ?? "authenticated";
            }
            return "anonymous";
        }
        catch
        {
            return "unknown";
        }
    }

    private static string GetMaskedUserId(HttpContext context)
    {
        try
        {
            // Hash or mask user identifier for correlation without exposing PII
            var user = context.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirst("sub")?.Value ?? user.Identity.Name;
                if (!string.IsNullOrEmpty(userId))
                {
                    // Simple masking - show only first 3 chars + hash
                    return userId.Length > 3 ?
                        $"{userId[..3]}***{userId.GetHashCode():X8}" :
                        $"***{userId.GetHashCode():X8}";
                }
            }
            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}
