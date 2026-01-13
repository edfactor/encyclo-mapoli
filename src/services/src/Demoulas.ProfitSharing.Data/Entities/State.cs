namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// Represents US states and territories used in profit sharing operations.
/// Provides a lookup table for state abbreviations and full names.
/// </summary>
public class State
{
    /// <summary>
    /// Two-character state or territory abbreviation (e.g., "MA", "NH", "GU", "PR").
    /// Primary key for the State entity.
    /// </summary>
    public required string Abbreviation { get; set; }

    /// <summary>
    /// Full name of the state or territory (e.g., "Massachusetts", "Guam", "Puerto Rico")
    /// </summary>
    public required string Name { get; set; }
}
