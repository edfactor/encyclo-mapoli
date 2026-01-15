namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service for retrieving vesting schedule information from the database.
/// </summary>
public interface IVestingScheduleService
{
    /// <summary>
    /// Gets the vesting percentage for a given schedule and years of service.
    /// </summary>
    /// <param name="scheduleId">The vesting schedule ID (1=Old Plan, 2=New Plan).</param>
    /// <param name="yearsOfService">The number of years of service.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The vesting percentage (0-100) for the given years of service.</returns>
    Task<decimal> GetVestingPercentAsync(int scheduleId, int yearsOfService, CancellationToken ct = default);

    /// <summary>
    /// Gets the effective year when the New Plan vesting schedule became active.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The year when the New Plan (2007) became effective.</returns>
    Task<int> GetNewPlanEffectiveYearAsync(CancellationToken ct = default);
}
