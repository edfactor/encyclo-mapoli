using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.Constants;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.Util.Extensions;
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

    private static string GetSessionId(HttpContext? httpContext)
    {
        if (httpContext?.Items.TryGetValue(Telemetry.SessionIdKey, out var itemSessionId) == true &&
            itemSessionId is string itemSessionIdStr && !string.IsNullOrEmpty(itemSessionIdStr))
        {
            return itemSessionIdStr;
        }

        if (httpContext?.Request.Cookies.TryGetValue(Telemetry.SessionIdKey, out var sessionId) == true &&
            !string.IsNullOrEmpty(sessionId))
        {
            return sessionId;
        }

        return "unknown";
    }

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

        var appUser = httpContext?.RequestServices?.GetService(typeof(Demoulas.Common.Contracts.Interfaces.IAppUser))
            as Demoulas.Common.Contracts.Interfaces.IAppUser;
        var userEmail = appUser?.Email ?? appUser?.UserName ?? "anonymous";

        var requestSize = EstimateObjectSize(request);

        if (!IsTestEnvironment(httpContext))
        {
            Demoulas.ProfitSharing.Common.Metrics.GlobalMeter.ApiRequests.Add(1,
                new(EndpointKey, endpointName),
                new(NavigationIdKey, endpoint.NavigationId.ToString()),
                new(UserRoleKey, userRole));

            EndpointTelemetry.RequestSizeBytes.Record(requestSize,
                new(EndpointKey, endpointName),
                new(NavigationIdKey, endpoint.NavigationId.ToString()));
        }

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

                logger?.LogInformation("Sensitive field accessed: {Field} by {UserEmail} ({UserRole}) in {Endpoint} - session: {SessionId}, correlation: {CorrelationId})",
                    field, userEmail, userRole, endpointName, sessionId, correlationId);
            }
        }

        logger?.LogDebug("Processing request in {Endpoint} by {UserEmail} ({UserRole}) - session: {SessionId}, correlation: {CorrelationId}, size: {RequestSize} bytes",
            endpointName, userEmail, userRole, sessionId, correlationId, requestSize);
    }

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

        var isTest = IsTestEnvironment(httpContext);
        var responseSize = isTest ? 0 : EstimateObjectSize(response);

        if (!isTest)
        {
            EndpointTelemetry.ResponseSizeBytes.Record(responseSize,
                new(EndpointKey, endpointName),
                new(NavigationIdKey, endpoint.NavigationId.ToString()));

            if (!isSuccess && !string.IsNullOrEmpty(errorType))
            {
                EndpointTelemetry.EndpointErrorsTotal.Add(1,
                    new(EndpointKey, endpointName),
                    new("error.type", errorType),
                    new(UserRoleKey, userRole));

                logger?.LogWarning("Endpoint error in {Endpoint}: {ErrorType} (correlation: {CorrelationId})",
                    endpointName, errorType, correlationId);
            }

            if (responseSize > 5_000_000)
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

        if (!IsTestEnvironment(httpContext))
        {
            EndpointTelemetry.EndpointErrorsTotal.Add(1,
                new(EndpointKey, endpointName),
                new("error.type", exception?.GetType().Name ?? "UnknownException"),
                new(UserRoleKey, userRole));
        }

        if (activity != null && exception != null)
        {
            activity.SetStatus(ActivityStatusCode.Error, exception.Message);
            activity.SetTag("error.type", exception.GetType().Name);
            activity.SetTag("error.message", exception.Message);
        }

        if (exception != null)
        {
            logger?.LogError(exception, "Unhandled exception in {Endpoint} for user role {UserRole} (correlation: {CorrelationId}): {ExceptionType}",
                endpointName, userRole, correlationId, exception.GetType().Name);
        }
    }

    [RequiresUnreferencedCode("This method uses reflection for size estimation, which is only called for telemetry in non-AOT scenarios")]
    [RequiresDynamicCode("JSON serialization may require dynamic code generation")]
    private static long EstimateObjectSize(object obj)
    {
        if (obj == null)
        {
            return 0;
        }

        var objType = obj.GetType();

        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(objType) &&
            objType != typeof(string))
        {
            try
            {
                var countProperty = objType.GetProperty("Count",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreCase);
                if (countProperty?.CanRead == true && countProperty.GetValue(obj) is int count)
                {
                    return (long)count * 500;
                }
            }
            catch
            {
                // Intentionally ignore and fall back to JSON serialization for size estimation.
            }
        }

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
            return 5120;
        }
    }

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
            endpoint.RecordRequestMetrics(httpContext, logger, request, sensitiveFields);

            var response = await executeFunc();

            endpoint.RecordResponseMetrics(httpContext, logger, response, isSuccess: true);

            return response;
        }
        catch (Exception ex)
        {
            endpoint.RecordException(httpContext, logger, ex, activity);
            throw;
        }
    }

    private static bool IsTestEnvironment(HttpContext? httpContext)
    {
        if (httpContext == null)
        {
            return true;
        }

        try
        {
            var hostEnvironment = httpContext.RequestServices.GetService(typeof(IHostEnvironment)) as IHostEnvironment;
            return hostEnvironment?.IsTestEnvironment() ?? true;
        }
        catch
        {
            return true;
        }
    }
}
