using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Extensions;

/// <summary>
/// Extension methods for adding consistent telemetry to all endpoints in a DRY manner.
/// Provides standardized patterns for activity creation, metrics recording, and sensitive data handling.
/// </summary>
public static class TelemetryExtensions
{
    private const string UserIdKey = "user.id";
    private const string UserRoleKey = "user.role";
    private const string EndpointKey = "endpoint.name";
    private const string NavigationIdKey = "navigation.id";
    private const string CorrelationIdKey = "correlation.id";

    /// <summary>
    /// Creates a standardized activity for an endpoint execution with common tags.
    /// </summary>
    /// <param name="endpoint">The endpoint instance (used for navigation ID and type name)</param>
    /// <param name="httpContext">The current HTTP context</param>
    /// <param name="operationName">Optional custom operation name, defaults to endpoint type name</param>
    /// <returns>The created activity or null if not enabled</returns>
    public static Activity? StartEndpointActivity(this IHasNavigationId endpoint, HttpContext httpContext, string? operationName = null)
    {
        var endpointName = operationName ?? endpoint.GetType().Name;
        var activity = EndpointTelemetry.ActivitySource.StartActivity($"endpoint.{endpointName}");

        if (activity != null)
        {
            var correlationId = httpContext.TraceIdentifier;
            var userId = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
            var userRole = httpContext.User?.FindFirst(ClaimTypes.Role)?.Value ?? "unknown";

            activity.SetTag(EndpointKey, endpointName);
            activity.SetTag(NavigationIdKey, endpoint.NavigationId.ToString());
            activity.SetTag(CorrelationIdKey, correlationId);
            activity.SetTag(UserIdKey, userId);
            activity.SetTag(UserRoleKey, userRole);
        }

        return activity;
    }

    /// <summary>
    /// Records request processing metrics and handles sensitive data logging.
    /// </summary>
    /// <param name="endpoint">The endpoint instance</param>
    /// <param name="httpContext">The current HTTP context</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="request">The request object (for size calculation)</param>
    /// <param name="sensitiveFields">Optional list of sensitive field names that were accessed</param>
    public static void RecordRequestMetrics<TRequest>(
        this IHasNavigationId endpoint,
        HttpContext httpContext,
        ILogger logger,
        TRequest request,
        params string[] sensitiveFields)
        where TRequest : notnull
    {
        var endpointName = endpoint.GetType().Name;
        var userRole = httpContext.User?.FindFirst(ClaimTypes.Role)?.Value ?? "unknown";
        var correlationId = httpContext.TraceIdentifier;

        // Calculate request size for monitoring
        var requestSize = EstimateObjectSize(request);

        // Record basic request metrics - use existing GlobalMeter API requests
        Demoulas.ProfitSharing.Common.Metrics.GlobalMeter.ApiRequests.Add(1,
            new(EndpointKey, endpointName),
            new(NavigationIdKey, endpoint.NavigationId.ToString()),
            new(UserRoleKey, userRole));

        // Record request size
        EndpointTelemetry.RequestSizeBytes.Record(requestSize,
            new(EndpointKey, endpointName),
            new(NavigationIdKey, endpoint.NavigationId.ToString()));

        // Record sensitive field access if any
        foreach (var field in sensitiveFields)
        {
            EndpointTelemetry.SensitiveFieldAccessTotal.Add(1,
                new("field", field),
                new(EndpointKey, endpointName),
                new(UserRoleKey, userRole));

            // Log sensitive field access with masked user info
            logger.LogInformation("Sensitive field accessed: {Field} by user role {UserRole} in {Endpoint} (correlation: {CorrelationId})",
                field, userRole, endpointName, correlationId);
        }

        // Structured log for request processing
        logger.LogDebug("Processing request in {Endpoint} for user role {UserRole} (correlation: {CorrelationId}, size: {RequestSize} bytes)",
            endpointName, userRole, correlationId, requestSize);
    }

    /// <summary>
    /// Records response metrics and handles large response detection.
    /// </summary>
    /// <param name="endpoint">The endpoint instance</param>
    /// <param name="httpContext">The current HTTP context</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="response">The response object (for size calculation)</param>
    /// <param name="isSuccess">Whether the response represents a successful operation</param>
    /// <param name="errorType">Optional error type for failure scenarios</param>
    public static void RecordResponseMetrics<TResponse>(
        this IHasNavigationId endpoint,
        HttpContext httpContext,
        ILogger logger,
        TResponse response,
        bool isSuccess = true,
        string? errorType = null)
        where TResponse : notnull
    {
        var endpointName = endpoint.GetType().Name;
        var userRole = httpContext.User?.FindFirst(ClaimTypes.Role)?.Value ?? "unknown";
        var correlationId = httpContext.TraceIdentifier;

        // Calculate response size
        var responseSize = EstimateObjectSize(response);

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

            logger.LogWarning("Endpoint error in {Endpoint}: {ErrorType} (correlation: {CorrelationId})",
                endpointName, errorType, correlationId);
        }

        // Detect and log large responses (potential security/performance concern)
        if (responseSize > 5_000_000) // 5MB threshold
        {
            EndpointTelemetry.LargeResponsesTotal.Add(1,
                new(EndpointKey, endpointName),
                new(UserRoleKey, userRole));

            logger.LogWarning("Large response detected in {Endpoint}: {ResponseSize} bytes for user role {UserRole} (correlation: {CorrelationId})",
                endpointName, responseSize, userRole, correlationId);
        }

        logger.LogDebug("Response completed for {Endpoint}: {ResponseSize} bytes, success: {IsSuccess} (correlation: {CorrelationId})",
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
        HttpContext httpContext,
        ILogger logger,
        Exception exception,
        Activity? activity = null)
    {
        var endpointName = endpoint.GetType().Name;
        var userRole = httpContext.User?.FindFirst(ClaimTypes.Role)?.Value ?? "unknown";
        var correlationId = httpContext.TraceIdentifier;

        // Record error metrics
        EndpointTelemetry.EndpointErrorsTotal.Add(1,
            new(EndpointKey, endpointName),
            new("error.type", exception.GetType().Name),
            new(UserRoleKey, userRole));

        // Set activity error status
        if (activity != null)
        {
            activity.SetStatus(ActivityStatusCode.Error, exception.Message);
            activity.SetTag("error.type", exception.GetType().Name);
            activity.SetTag("error.message", exception.Message);
        }

        // Structured error logging
        logger.LogError(exception, "Unhandled exception in {Endpoint} for user role {UserRole} (correlation: {CorrelationId}): {ExceptionType}",
            endpointName, userRole, correlationId, exception.GetType().Name);
    }

    /// <summary>
    /// Helper method to mask sensitive data in log messages.
    /// </summary>
    /// <param name="value">The value to mask</param>
    /// <param name="fieldName">The field name (for context-aware masking)</param>
    /// <returns>Masked value safe for logging</returns>
    public static string MaskSensitiveValue(string? value, string fieldName)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "null";
        }

        return fieldName.ToLowerInvariant() switch
        {
            "ssn" or "socialsecuritynumber" => value.Length >= 4 ? $"XXX-XX-{value[^4..]}" : "XXX-XX-XXXX",
            "email" => MaskEmail(value),
            "phone" or "phonenumber" => value.Length >= 4 ? $"XXX-XXX-{value[^4..]}" : "XXX-XXX-XXXX",
            _ => value.Length > 4 ? $"{value[..2]}***{value[^2..]}" : "****"
        };
    }

    private static string MaskEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        if (atIndex <= 0)
        {
            return "***@***.***";
        }

        var localPart = email[..atIndex];
        var domainPart = email[(atIndex + 1)..];

        var maskedLocal = localPart.Length > 2 ? $"{localPart[0]}***{localPart[^1]}" : "***";
        return $"{maskedLocal}@{domainPart}";
    }

    /// <summary>
    /// Estimates the size of an object for metrics purposes.
    /// Uses JSON serialization as a reasonable approximation of payload size.
    /// </summary>
    private static long EstimateObjectSize(object obj)
    {
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
            // Fallback to a reasonable estimate if serialization fails
            return 1024; // 1KB default estimate
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
    public static async Task<TResponse> ExecuteWithTelemetry<TRequest, TResponse>(
        this IHasNavigationId endpoint,
        HttpContext httpContext,
        ILogger logger,
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
}
