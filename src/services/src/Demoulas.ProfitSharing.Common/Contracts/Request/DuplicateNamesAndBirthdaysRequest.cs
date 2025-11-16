using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request DTO for Duplicate Names and Birthdays report with optional filtering of fictional SSN pairs.
/// </summary>
public record DuplicateNamesAndBirthdaysRequest : ProfitYearRequest
{
    /// <summary>
    /// If true, includes records with fictional SSNs in the results.
    /// If false (default), filters out all records marked with fictional SSNs.
    /// </summary>
    [DefaultValue(false)]
    public bool IncludeFictionalSsnPairs { get; init; }
}
