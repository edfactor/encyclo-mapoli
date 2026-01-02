namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Interface for requests that include an employee badge number.
/// Employee badge numbers must be integers between 2 and 11 digits (10 to 99,999,999,999).
/// </summary>
public interface IEmployeeBadgeNumberRequest
{
    /// <summary>
    /// Gets or sets the employee badge number. Must be between 2 and 11 digits (10 to 99,999,999,999).
    /// </summary>
    int EmployeeBadgeNumber { get; set; }
}
