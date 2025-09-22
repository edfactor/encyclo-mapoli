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
}
