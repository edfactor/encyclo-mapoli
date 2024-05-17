using System.Diagnostics;
using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Demoulas.ProfitSharing.Api.Extensions;

/// <summary>
/// Provides extensions for converting exceptions to ProblemDetails object that is then sent to the consumer
/// </summary>
internal static class ExceptionExtensions
{
    private const string _typeString = "https://www.shopmarketbasket.com/about-us/contact-us";
    public static ProblemDetails ToProblemDetails(this Exception ex, string? title = null, string? details = null, string? instance = null,
        string? type = null)
    {
        return new ProblemDetails
        {
            Title = title ?? ex.Message,
            Status = (int)HttpStatusCode.InternalServerError,
            Detail = details ?? (!Debugger.IsAttached ? ex.Message : ex.ToString()),
            Instance = instance,
            Type = type ?? _typeString,
            Extensions =
                {
                    {
                        "timestamp", DateTimeOffset.UtcNow
                    }
                }
        };
    }

    public static ProblemDetails ToProblemDetails(this BadHttpRequestException ex, string? title = null, string? details = null, string? instance = null, string? type = null)
    {
        return new ProblemDetails
        {
            Title = title ?? ex.Message,
            Status = ex.StatusCode,
            Detail = details ?? (!Debugger.IsAttached ? ex.Message : ex.ToString()),
            Instance = instance,
            Type = type ?? _typeString,
            Extensions =
                {
                    {
                        "timestamp", DateTimeOffset.UtcNow
                    }
                }
        };
    }

    public static ValidationProblemDetails ToProblemDetails(this ValidationException ex, string? title = null, string? details = null, string? instance = null, string? type = null)
    {
        Dictionary<string, string[]> errorsDictionary = ex.Errors.GroupBy(f => f.PropertyName)
            .ToDictionary(
                e => e.Key,
                e => e.Select(m => m.ErrorMessage).ToArray());


        return new ValidationProblemDetails
        {
            Title = title ?? ex.Message,
            Status = (int)HttpStatusCode.BadRequest,
            Detail = details ?? (!Debugger.IsAttached ? ex.Message : ex.ToString()),
            Instance = instance,
            Type = type ?? _typeString,
            Extensions =
                {
                    {
                        "timestamp", DateTimeOffset.UtcNow
                    }
                },
            Errors = errorsDictionary
        };
    }
}
