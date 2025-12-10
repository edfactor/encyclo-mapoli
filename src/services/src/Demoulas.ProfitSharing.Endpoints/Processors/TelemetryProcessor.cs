using System.Diagnostics;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Processors;

/// <summary>
/// FastEndpoints processor that automatically adds telemetry to all endpoints.
/// This provides consistent telemetry collection across all API endpoints without requiring
/// manual instrumentation in each endpoint implementation.
/// </summary>
public class TelemetryProcessor : IPreProcessor, IPostProcessor
{
    private readonly ILogger<TelemetryProcessor> _logger;

    public TelemetryProcessor(ILogger<TelemetryProcessor> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Pre-process hook that starts telemetry tracking before endpoint execution.
    /// </summary>
    public Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        if (context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<EndpointDefinition>() is { } def &&
            def.EndpointType.IsAssignableTo(typeof(IHasNavigationId)) &&
            Activator.CreateInstance(def.EndpointType, args: [(short)0]) is IHasNavigationId endpoint)
        {
            // Start activity and store it in HttpContext for later retrieval
            var activity = endpoint.StartEndpointActivity(context.HttpContext);
            if (activity != null)
            {
                context.HttpContext.Items["TelemetryActivity"] = activity;
                context.HttpContext.Items["TelemetryEndpoint"] = endpoint;
                context.HttpContext.Items["TelemetryStartTime"] = Stopwatch.GetTimestamp();
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Post-process hook that completes telemetry tracking after endpoint execution.
    /// </summary>
    public Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
    {
        // Retrieve telemetry context from HttpContext
        if (context.HttpContext.Items.TryGetValue("TelemetryActivity", out object? activityObj) &&
            context.HttpContext.Items.TryGetValue("TelemetryEndpoint", out object? endpointObj) &&
            context.HttpContext.Items.TryGetValue("TelemetryStartTime", out object? startTimeObj) &&
            activityObj is Activity activity &&
            endpointObj is IHasNavigationId endpoint &&
            startTimeObj is long startTime)
        {
            try
            {
                // Calculate execution duration
                long endTime = Stopwatch.GetTimestamp();
                double elapsedMs = (double)(endTime - startTime) / Stopwatch.Frequency * 1000;

                // Get session ID and user info for correlation across requests
                var sessionId = GetSessionId(context.HttpContext);
                var appUser = context.HttpContext.RequestServices?.GetService(typeof(Demoulas.Common.Contracts.Interfaces.IAppUser))
                    as Demoulas.Common.Contracts.Interfaces.IAppUser;
                var userEmail = appUser?.Email ?? appUser?.UserName ?? "unknown";

                // Record execution duration
                Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.EndpointDurationMs.Record(elapsedMs,
                    new KeyValuePair<string, object?>("endpoint.name", endpoint.GetType().Name),
                    new KeyValuePair<string, object?>("navigation.id", endpoint.NavigationId.ToString()),
                    new KeyValuePair<string, object?>("session.id", sessionId),
                    new KeyValuePair<string, object?>("user.id", userEmail));

                // Determine if the response was successful based on HTTP status code
                bool isSuccess = context.HttpContext.Response.StatusCode is >= 200 and < 400;
                string? errorType = isSuccess ? null : $"HTTP_{context.HttpContext.Response.StatusCode}";

                // Get user role once for all metrics
                string userRole = context.HttpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "unknown";

                if (!isSuccess)
                {
                    // Record error metrics
                    Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.EndpointErrorsTotal.Add(1,
                        new KeyValuePair<string, object?>("endpoint.name", endpoint.GetType().Name),
                        new KeyValuePair<string, object?>("error.type", errorType ?? "unknown"),
                        new KeyValuePair<string, object?>("user.role", userRole),
                        new KeyValuePair<string, object?>("user.id", userEmail),
                        new KeyValuePair<string, object?>("session.id", sessionId));

                    // Set activity error status
                    activity.SetStatus(ActivityStatusCode.Error, $"HTTP {context.HttpContext.Response.StatusCode}");
                    activity.SetTag("error.type", errorType);
                    activity.SetTag("http.status_code", context.HttpContext.Response.StatusCode);

                    _logger.LogWarning("Endpoint execution failed: {Endpoint} returned {StatusCode} by {UserEmail} ({UserRole}) - session: {SessionId}, correlation: {CorrelationId}, duration: {DurationMs}ms",
                        endpoint.GetType().Name, context.HttpContext.Response.StatusCode, userEmail, userRole, sessionId, context.HttpContext.TraceIdentifier, elapsedMs);
                }
                else
                {
                    // Record successful execution
                    activity.SetTag("http.status_code", context.HttpContext.Response.StatusCode);

                    _logger.LogDebug("Endpoint execution completed: {Endpoint} returned {StatusCode} by {UserEmail} ({UserRole}) - session: {SessionId}, correlation: {CorrelationId}, duration: {DurationMs}ms",
                        endpoint.GetType().Name, context.HttpContext.Response.StatusCode, userEmail, userRole, sessionId, context.HttpContext.TraceIdentifier, elapsedMs);
                }

                // Record user activity metrics (aggregated by role for low cardinality)
                Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.UserActivityTotal.Add(1,
                    new KeyValuePair<string, object?>("endpoint.category", GetEndpointCategory(endpoint.GetType().Name)),
                    new KeyValuePair<string, object?>("user.role", userRole),
                    new KeyValuePair<string, object?>("success", isSuccess.ToString().ToLowerInvariant()));

                // Estimate response size if available
                if (context.HttpContext.Response.ContentLength.HasValue)
                {
                    long responseSize = context.HttpContext.Response.ContentLength.Value;
                    Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.ResponseSizeBytes.Record(responseSize,
                        new KeyValuePair<string, object?>("endpoint.name", endpoint.GetType().Name),
                        new KeyValuePair<string, object?>("navigation.id", endpoint.NavigationId.ToString()));

                    // Check for large responses (potential security concern)
                    if (responseSize > 5_000_000) // 5MB threshold
                    {
                        Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.LargeResponsesTotal.Add(1,
                            new KeyValuePair<string, object?>("endpoint.name", endpoint.GetType().Name),
                            new KeyValuePair<string, object?>("user.role", userRole));

                        _logger.LogWarning("Large response detected: {Endpoint} returned {ResponseSize} bytes for user role {UserRole} (correlation: {CorrelationId})",
                            endpoint.GetType().Name, responseSize, userRole, context.HttpContext.TraceIdentifier);
                    }
                }
            }
            finally
            {
                // Always dispose the activity
                activity.Dispose();
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Categorizes endpoints for low-cardinality metrics.
    /// </summary>
    private static string GetEndpointCategory(string endpointName)
    {
        return endpointName.ToLowerInvariant() switch
        {
            var name when name.Contains("master") => "master-inquiry",
            var name when name.Contains("navigation") => "navigation",
            var name when name.Contains("yearend") => "year-end",
            var name when name.Contains("lookup") => "lookup",
            var name when name.Contains("beneficiary") => "beneficiary",
            var name when name.Contains("itoperations") => "it-operations",
            var name when name.Contains("certificate") => "certificate",
            var name when name.Contains("demographics") => "demographics",
            var name when name.Contains("security") => "security",
            var name when name.Contains("health") => "health",
            _ => "other"
        };
    }

    /// <summary>
    /// Retrieves the session ID from cookies for user journey tracking across requests.
    /// </summary>
    private static string GetSessionId(HttpContext? context)
    {
        const string sessionCookieName = "ps-session-id";

        // First, try to get from HttpContext.Items (set by middleware in same request)
        if (context?.Items.TryGetValue(sessionCookieName, out var itemSessionId) == true &&
            itemSessionId is string itemSessionIdStr && !string.IsNullOrEmpty(itemSessionIdStr))
        {
            return itemSessionIdStr;
        }

        // Fallback: try to get from request cookies (for subsequent requests)
        if (context?.Request.Cookies.TryGetValue(sessionCookieName, out var sessionId) == true &&
            !string.IsNullOrEmpty(sessionId))
        {
            return sessionId;
        }

        return "unknown";
    }
}
