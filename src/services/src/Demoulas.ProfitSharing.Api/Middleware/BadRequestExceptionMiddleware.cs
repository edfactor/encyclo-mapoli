using System.Net;
using Demoulas.ProfitSharing.Api.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Demoulas.ProfitSharing.Api.Middleware;

/// <summary>
/// Global exception handler that converts exceptions and passes them up to the caller as a consistent ProblemDetails object
/// </summary>
public class BadRequestExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public BadRequestExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (BadHttpRequestException ex)
        {
            await HandleBadRequestExceptionAsync(context, ex).ConfigureAwait(false);
        }
        catch (ValidationException ex)
        {
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleUnauthorizedAccessExceptionAsync(context, ex).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
    }

    private static Task HandleBadRequestExceptionAsync(HttpContext context, BadHttpRequestException exception)
    {
        ProblemDetails problemDetails = exception.ToProblemDetails(instance: context.Request.Path);
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = exception.StatusCode;
        return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(problemDetails));
    }

    private static Task HandleUnauthorizedAccessExceptionAsync(HttpContext context, UnauthorizedAccessException exception)
    {
        context.Response.ContentType = "application/text";
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        return context.Response.WriteAsync(exception.Message);
    }

    private static Task HandleExceptionAsync(HttpContext context, ValidationException exception)
    {
        ValidationProblemDetails problemDetails = exception.ToProblemDetails(instance: context.Request.Path);
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(problemDetails));
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        ProblemDetails problemDetails = exception.ToProblemDetails(instance: context.Request.Path);
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(problemDetails));
    }
}
