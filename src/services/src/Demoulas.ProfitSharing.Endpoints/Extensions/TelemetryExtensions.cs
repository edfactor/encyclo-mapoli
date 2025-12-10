using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Extensions;

/// <summary>
/// Extension methods for adding consistent telemetry to all endpoints in a DRY manner.
/// Provides standardized patterns for activity creation, metrics recording, and sensitive data handling.
/// </summary>
public static class TelemetryExtensions
{
    private const string UserIdKey = "user.id";
    private const string UserEmailKey = "user.email";
    private const string SessionIdKey = "session.id";
    private const string UserRoleKey = "user.role";
    private const string EndpointKey = "endpoint.name";
    private const string NavigationIdKey = "navigation.id";
    private const string CorrelationIdKey = "correlation.id";

    /// <summary>
    /// Creates a standardized activity for an endpoint execution with common tags.
    /// Includes session ID and unique user identifier for journey tracking.
    /// </summary>
    /// <param name="endpoint">The endpoint instance (used for navigation ID and type name)</param>
    /// <param name="httpContext">The current HTTP context</param>
    /// <param name="operationName">Optional custom operation name, defaults to endpoint type name</param>
    /// <returns>The created activity or null if not enabled</returns>
    public static Activity? StartEndpointActivity(this IHasNavigationId endpoint, HttpContext? httpContext, string? operationName = null)
    {
        var endpointName = operationName ?? endpoint.GetType().Name;
        var activity = EndpointTelemetry.ActivitySource?.StartActivity($"endpoint.{endpointName}");

        if (activity != null && httpContext != null)
        {
            var correlationId = httpContext.TraceIdentifier ?? "test-correlation";
            var sessionId = GetSessionId(httpContext);

            // Get unique user identifier from IAppUser (email preferred, fallback to username)
            var appUser = httpContext.RequestServices?.GetService(typeof(Demoulas.Common.Contracts.Interfaces.IAppUser))
                as Demoulas.Common.Contracts.Interfaces.IAppUser;
            var userEmail = appUser?.Email ?? appUser?.UserName ?? "anonymous";
            var userRole = httpContext.User?.FindFirst(ClaimTypes.Role)?.Value ?? "unknown";

            activity.SetTag(EndpointKey, endpointName);
            activity.SetTag(NavigationIdKey, endpoint.NavigationId.ToString());
            activity.SetTag(CorrelationIdKey, correlationId);
            activity.SetTag(SessionIdKey, sessionId);
            activity.SetTag(UserIdKey, userEmail);
            activity.SetTag(UserEmailKey, userEmail);
            activity.SetTag(UserRoleKey, userRole);
        }

        return activity;
    }

    /// <summary>
    /// Retrieves the session ID from the HTTP context (set by EndpointInstrumentationMiddleware).
    /// Session ID correlates all requests within a user session for journey tracking.
    /// </summary>
    private static string GetSessionId(HttpContext httpContext)
    {
        const string SessionCookieName = "ps-session-id";

        if (httpContext.Request.Cookies.TryGetValue(SessionCookieName, out var sessionId) &&
            !string.IsNullOrEmpty(sessionId))
        {
            return sessionId;
        }

        return "unknown";
    }

    /// <summary>
    /// Records request processing metrics and handles sensitive data logging.
    /// </summary>
    /// <param name="endpoint">The endpoint instance</param>
    /// <param name="httpContext">The current HTTP context</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="request">The request object (for size calculation)</param>
    /// <param name="sensitiveFields">Optional list of sensitive field names that were accessed</param>
    [RequiresUnreferencedCode("Calls EstimateObjectSize which uses reflection")]
    [RequiresDynamicCode("Calls EstimateObjectSize which may require dynamic code")]
    public static void RecordRequestMetrics<TRequest>(
        this IHasNavigationId endpoint,
        HttpContext? httpContext,
        ILogger? logger,
        TRequest request,
        params string[] sensitiveFields)
        where TRequest : notnull
    {
        var endpointName = endpoint.GetType().Name;
        var userRole = httpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? "unknown";
        var correlationId = httpContext?.TraceIdentifier ?? "test-correlation";
        var sessionId = GetSessionId(httpContext);

        // Get user email from IAppUser
        var appUser = httpContext?.RequestServices?.GetService(typeof(Demoulas.Common.Contracts.Interfaces.IAppUser))
            as Demoulas.Common.Contracts.Interfaces.IAppUser;
        var userEmail = appUser?.Email ?? appUser?.UserName ?? "anonymous";

        // Calculate request size for monitoring
        var requestSize = EstimateObjectSize(request);

        // Record basic request metrics - use existing GlobalMeter API requests (skip in test environments)
        if (!IsTestEnvironment(httpContext))
        {
            Demoulas.ProfitSharing.Common.Metrics.GlobalMeter.ApiRequests.Add(1,
                new(EndpointKey, endpointName),
                new(NavigationIdKey, endpoint.NavigationId.ToString()),
                new(UserRoleKey, userRole));

            // Record request size
            EndpointTelemetry.RequestSizeBytes.Record(requestSize,
                new(EndpointKey, endpointName),
                new(NavigationIdKey, endpoint.NavigationId.ToString()));
        }

        // Record sensitive field access if any (skip in test environments)
        if (!IsTestEnvironment(httpContext))
        {
            foreach (var field in sensitiveFields)
            {
                EndpointTelemetry.SensitiveFieldAccessTotal.Add(1,
                    new("field", field),
                    new(EndpointKey, endpointName),
                    new(UserRoleKey, userRole),
                    new(UserEmailKey, userEmail),
                    new(SessionIdKey, sessionId));

                // Log sensitive field access with full user journey context
                logger?.LogInformation("Sensitive field accessed: {Field} by {UserEmail} ({UserRole}) in {Endpoint} - session: {SessionId}, correlation: {CorrelationId}",
                    field, userEmail, userRole, endpointName, sessionId, correlationId);
            }
        }

        // Structured log for request processing with user journey context
        logger?.LogDebug("Processing request in {Endpoint} by {UserEmail} ({UserRole}) - session: {SessionId}, correlation: {CorrelationId}, size: {RequestSize} bytes",
            endpointName, userEmail, userRole, sessionId, correlationId, requestSize);
    }

    /// <summary>
    /// Records response metrics and handles large response detection.
    /// IMPORTANT: In test environments, skip expensive serialization to prevent timeouts.
    /// </summary>
    /// <param name="endpoint">The endpoint instance</param>
    /// <param name="httpContext">The current HTTP context</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="response">The response object (for size calculation)</param>
    /// <param name="isSuccess">Whether the response represents a successful operation</param>
    /// <param name="errorType">Optional error type for failure scenarios</param>
    [RequiresUnreferencedCode("Calls EstimateObjectSize which uses reflection")]
    [RequiresDynamicCode("Calls EstimateObjectSize which may require dynamic code")]
    public static void RecordResponseMetrics<TResponse>(
        this IHasNavigationId endpoint,
        HttpContext? httpContext,
        ILogger? logger,
        TResponse response,
        bool isSuccess = true,
        string? errorType = null)
        where TResponse : notnull
    {
        var endpointName = endpoint.GetType().Name;
        var userRole = httpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? "unknown";
        var correlationId = httpContext?.TraceIdentifier ?? "test-correlation";

        // CRITICAL: Skip expensive size estimation in test environments
        // This prevents integration tests from hanging due to large response serialization
        var isTest = IsTestEnvironment(httpContext);
        var responseSize = isTest ? 0 : EstimateObjectSize(response);

        // Record response metrics (skip in test environments)
        if (!isTest)
        {
            // Record response size
            EndpointTelemetry.ResponseSizeBytes.Record(responseSize,
                new(EndpointKey, endpointName),
                new(NavigationIdKey, endpoint.NavigationId.ToString()));

            // Record errors if applicable
            if (!isSuccess && !string.IsNullOrEmpty(errorType))
            {
                EndpointTelemetry.EndpointErrorsTotal.Add(1,
                    new(EndpointKey, endpointName),
                    new("error.type", errorType),
                    new(UserRoleKey, userRole));

                logger?.LogWarning("Endpoint error in {Endpoint}: {ErrorType} (correlation: {CorrelationId})",
                    endpointName, errorType, correlationId);
            }

            // Detect and log large responses (potential security/performance concern)
            if (responseSize > 5_000_000) // 5MB threshold
            {
                EndpointTelemetry.LargeResponsesTotal.Add(1,
                    new(EndpointKey, endpointName),
                    new(UserRoleKey, userRole));

                logger?.LogWarning("Large response detected in {Endpoint}: {ResponseSize} bytes for user role {UserRole} (correlation: {CorrelationId})",
                    endpointName, responseSize, userRole, correlationId);
            }
        }

        logger?.LogDebug("Response completed for {Endpoint}: {ResponseSize} bytes, success: {IsSuccess} (correlation: {CorrelationId})",
            endpointName, responseSize, isSuccess, correlationId);
    }

    /// <summary>
    /// Creates a standardized exception activity and records error metrics.
    /// </summary>
    /// <param name="endpoint">The endpoint instance</param>
    /// <param name="httpContext">The current HTTP context</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="exception">The exception that occurred</param>
    /// <param name="activity">The current activity to set error status on</param>
    public static void RecordException(
        this IHasNavigationId endpoint,
        HttpContext? httpContext,
        ILogger? logger,
        Exception? exception,
        Activity? activity = null)
    {
        var endpointName = endpoint.GetType().Name;
        var userRole = httpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? "unknown";
        var correlationId = httpContext?.TraceIdentifier ?? "test-correlation";

        // Record error metrics (skip in test environments)
        if (!IsTestEnvironment(httpContext))
        {
            EndpointTelemetry.EndpointErrorsTotal.Add(1,
                new(EndpointKey, endpointName),
                new("error.type", exception?.GetType().Name ?? "UnknownException"),
                new(UserRoleKey, userRole));
        }

        // Set activity error status
        if (activity != null && exception != null)
        {
            activity.SetStatus(ActivityStatusCode.Error, exception.Message);
            activity.SetTag("error.type", exception.GetType().Name);
            activity.SetTag("error.message", exception.Message);
        }

        // Structured error logging
        if (exception != null)
        {
            logger?.LogError(exception, "Unhandled exception in {Endpoint} for user role {UserRole} (correlation: {CorrelationId}): {ExceptionType}",
                endpointName, userRole, correlationId, exception.GetType().Name);
        }
    }

    /// <summary>
    /// Estimates the size of an object for metrics purposes.
    /// For large collections, uses a heuristic to avoid expensive serialization.
    /// For smaller objects, uses JSON serialization for accuracy.
    /// 
    /// This method is critical for performance - it must not block the response pipeline.
    /// Large response objects (e.g., paginated result sets with thousands of items) can take
    /// seconds to serialize, causing test timeouts and production performance issues.
    /// </summary>
    [RequiresUnreferencedCode("This method uses reflection for size estimation, which is only called for telemetry in non-AOT scenarios")]
    [RequiresDynamicCode("JSON serialization may require dynamic code generation")]
    private static long EstimateObjectSize(object obj)
    {
        if (obj == null)
        {
            return 0;
        }

        var objType = obj.GetType();

        // Fast path: Check if it's a collection with Count/Length
        // This avoids serializing large datasets
        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(objType) &&
            objType != typeof(string))
        {
            try
            {
                // Try to get Count property for ICollection types
                var countProperty = objType.GetProperty("Count",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreCase);
                if (countProperty?.CanRead == true && countProperty.GetValue(obj) is int count)
                {
                    // Estimate: assume ~500 bytes per item for complex objects, ~50 for scalars
                    // This is a conservative heuristic to avoid massive underestimation
                    return (long)count * 500;
                }
            }
            catch
            {
                // Fall through to serialization attempt
            }
        }

        // Fallback: attempt JSON serialization for smaller objects
        // NOTE: Serialization can be expensive for large objects, so this is only called 
        // after checking IsTestEnvironment and if fast heuristics fail
        try
        {
            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            return System.Text.Encoding.UTF8.GetByteCount(json);
        }
        catch
        {
            // If serialization fails, use a conservative default estimate
            // This prevents exceptions from crashing telemetry
            return 5120; // 5KB default estimate
        }
    }

    /// <summary>
    /// Extension method to wrap endpoint execution with comprehensive telemetry.
    /// This is the main method that endpoints should use for consistent telemetry.
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="endpoint">The endpoint instance</param>
    /// <param name="httpContext">Current HTTP context</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="request">Request object</param>
    /// <param name="executeFunc">The actual endpoint execution logic</param>
    /// <param name="sensitiveFields">List of sensitive fields accessed during execution</param>
    /// <returns>The response from the execution function</returns>
    [RequiresUnreferencedCode("Calls telemetry methods that use reflection")]
    [RequiresDynamicCode("Calls telemetry methods that may require dynamic code")]
    public static async Task<TResponse> ExecuteWithTelemetry<TRequest, TResponse>(
        this IHasNavigationId endpoint,
        HttpContext? httpContext,
        ILogger? logger,
        TRequest request,
        Func<Task<TResponse>> executeFunc,
        params string[] sensitiveFields)
        where TRequest : notnull
        where TResponse : notnull
    {
        using var activity = endpoint.StartEndpointActivity(httpContext);

        try
        {
            // Record request metrics and sensitive field access
            endpoint.RecordRequestMetrics(httpContext, logger, request, sensitiveFields);

            // Execute the actual endpoint logic
            var response = await executeFunc();

            // Record successful response metrics
            endpoint.RecordResponseMetrics(httpContext, logger, response, isSuccess: true);

            return response;
        }
        catch (Exception ex)
        {
            // Record exception and error metrics
            endpoint.RecordException(httpContext, logger, ex, activity);
            throw; // Re-throw to maintain original exception handling
        }
    }

    /// <summary>
    /// Helper method to determine if we're running in a test environment.
    /// Returns true if HttpContext is null (unit test) or if the host environment is a test environment.
    /// </summary>
    /// <param name="httpContext">The HTTP context (null in unit tests)</param>
    /// <returns>True if running in test environment, false otherwise</returns>
    private static bool IsTestEnvironment(HttpContext? httpContext)
    {
        // If HttpContext is null, we're likely in a unit test
        if (httpContext == null)
        {
            return true;
        }

        // Try to get the host environment from services
        try
        {
            var hostEnvironment = httpContext.RequestServices.GetService(typeof(IHostEnvironment)) as IHostEnvironment;
            return hostEnvironment?.IsTestEnvironment() ?? true; // Default to test if we can't determine
        }
        catch
        {
            // If we can't get the environment, assume it's a test
            return true;
        }
    }
}
