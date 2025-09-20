using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Demoulas.ProfitSharing.Common.Contracts;

public sealed record Error
{
    private Error(int code, string description, Dictionary<string, string[]>? validationErrors = null)
    {
        Code = code;
        Description = description;
        ValidationErrors = validationErrors ?? new Dictionary<string, string[]>();
    }

    public string Description { get; init; }
    public int Code { get; init; }
    public Dictionary<string, string[]> ValidationErrors { get; init; }

    public static Error Validation(Dictionary<string, string[]> errors) =>
        new(400, "Validation error", errors);

    public static Error EmployeeNotFound => new(100, "Employee not found");
    public static Error CalendarYearNotFound => new(101, "Calendar year not found");

    public static implicit operator ProblemDetails(Error error)
    {
        return new ProblemDetails
        {
            Title = "Validation Failed",
            Detail = error.Description,
            Status = (int)HttpStatusCode.BadRequest,
            Extensions = { ["errors"] = error.ValidationErrors },
        };
    }
}
