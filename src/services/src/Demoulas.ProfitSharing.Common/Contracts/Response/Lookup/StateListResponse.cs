using System.ComponentModel.DataAnnotations;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

/// <summary>
/// Response model for state list lookup containing state abbreviation and full name
/// </summary>
public sealed record StateListResponse
{
    /// <summary>
    /// Two-character state abbreviation (e.g., "MA", "NH")
    /// </summary>
    [Key]
    public required string Abbreviation { get; set; }

    /// <summary>
    /// Full state name (e.g., "Massachusetts", "New Hampshire")
    /// </summary>
    public required string Name { get; set; }
}
