namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Interface for requests that include a badge number.
/// Badge numbers must be integers between 3 and 11 digits (100 to 99,999,999,999).
/// </summary>
public interface IBadgeNumberRequest
{
    /// <summary>
    /// Gets or sets the badge number. Must be between 3 and 11 digits (100 to 99,999,999,999).
    /// </summary>
    int BadgeNumber { get; }
}
