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
    public static Error DistributionNotFound => new(102, "Distribution not found");
    public static Error BadgeNumberNotFound => new(103, "Badge number not found");
    // Generic entity not found (dynamic description) - use only when a more specific constant does not exist
    public static Error EntityNotFound(string entityName) => new(104, $"{entityName} not found");
    public static Error NoPayProfitsDataAvailable => new(105, "No PayProfits data available in the system");

    // Military contribution errors
    public static Error MilitaryContributionDuplicate => new(111, "A regular military contribution already exists for this year");
    public static Error MilitaryContributionInvalidYear => new(112, "Military contribution year is invalid");
    public static Error MilitaryContributionInvalidAmount => new(113, "Military contribution amount must be greater than zero");
    public static Error MilitaryContributionEmployeeNotActive => new(114, "Employee is not active as of the contribution date");
    public static Error MilitaryContributionEmployeeNotEligible => new(115, "Employee is not eligible for military contributions");

    // Merge operation errors
    public static Error SourceDemographicNotFound => new(106, "Source demographic not found");
    public static Error DestinationDemographicNotFound => new(107, "Destination demographic not found");
    public static Error BothDemographicsNotFound => new(108, "Both source and destination demographics not found");
    public static Error SameDemographicMerge => new(109, "Cannot merge demographic with itself");
    public static Error MergeOperationFailed(string message) => new(110, $"Merge operation failed: {message}");

    // Unexpected error wrapper (message captured). Prefer logging full exception separately.
    public static Error Unexpected(string message) => new(900, message);

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
