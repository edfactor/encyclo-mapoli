namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IEmployeeLookupService
{
    Task<bool> BadgeExistsAsync(int badgeNumber, CancellationToken cancellationToken = default);
    /// <summary>
    /// Returns the earliest known hire date for the employee identified by <paramref name="badgeNumber"/>.
    /// If the employee is not found, returns <c>null</c>.
    /// </summary>
    Task<DateOnly?> GetEarliestHireDateAsync(int badgeNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the employee's date of birth if available; otherwise <c>null</c>.
    /// </summary>
    Task<DateOnly?> GetDateOfBirthAsync(int badgeNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the employee's employment status id as-of the given date.
    /// If a termination date is on or before <paramref name="asOfDate"/>, returns 't'.
    /// Returns null when the employee is not found.
    /// </summary>
    Task<char?> GetEmploymentStatusIdAsOfAsync(int badgeNumber, DateOnly asOfDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns whether the employee is Active as-of the given date. Null if employee not found or status unknown.
    /// </summary>
    Task<bool?> IsActiveAsOfAsync(int badgeNumber, DateOnly asOfDate, CancellationToken cancellationToken = default);
}
