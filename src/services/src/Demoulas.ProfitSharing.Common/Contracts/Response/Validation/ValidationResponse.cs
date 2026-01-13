using System;
using System.Collections.Generic;
using System.Text;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Validation;

public record ValidationResponse : IProfitYearRequest
{
    /// <summary>
    /// The profit year being validated
    /// </summary>
    public short ProfitYear { get; set; }

    /// <summary>
    /// Whether all cross-reference validations passed
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Overall summary message
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Grouped validation results by category (Distributions, Forfeitures, Contributions, etc.)
    /// </summary>
    public List<CrossReferenceValidationGroup> ValidationGroups { get; init; } = new();

    /// <summary>
    /// Count of total validations performed
    /// </summary>
    public int TotalValidations { get; init; }

    /// <summary>
    /// Count of validations that passed
    /// </summary>
    public int PassedValidations { get; init; }

    /// <summary>
    /// Count of validations that failed
    /// </summary>
    public int FailedValidations { get; init; }

    /// <summary>
    /// List of all reports that were validated against
    /// </summary>
    public List<string> ValidatedReports { get; init; } = new();

    /// <summary>
    /// When this validation was performed
    /// </summary>
    public DateTimeOffset ValidatedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// List of critical issues that must be resolved before proceeding
    /// </summary>
    public List<string> CriticalIssues { get; init; } = new();

    public static ValidationResponse ResponseExample()
    {
        return new ValidationResponse
        {
            ProfitYear = 2024,
            IsValid = true,
            Message = "All cross-reference validations passed",
            ValidationGroups = new List<CrossReferenceValidationGroup>
            {
                CrossReferenceValidationGroup.ResponseExample()
            },
            TotalValidations = 10,
            PassedValidations = 10,
            FailedValidations = 0,
            ValidatedReports = new List<string> { "PAY443", "QPAY129", "PAY444" },
            ValidatedAt = DateTimeOffset.UtcNow,
            CriticalIssues = new List<string>()
        };
    }

    /// <summary>
    /// Additional warnings that don't block the operation but should be reviewed
    /// </summary>
    public List<string> Warnings { get; init; } = new();
}
