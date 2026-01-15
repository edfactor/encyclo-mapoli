namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// Represents a specific vesting percentage for a given number of years of service
/// within a vesting schedule.
/// </summary>
/// <remarks>
/// Each detail row maps a years-of-service value to a vesting percentage.
/// Example for New Plan:
/// - YearsOfService=0, VestingPercent=0
/// - YearsOfService=1, VestingPercent=0
/// - YearsOfService=2, VestingPercent=20
/// - YearsOfService=3, VestingPercent=40
/// - YearsOfService=4, VestingPercent=60
/// - YearsOfService=5, VestingPercent=80
/// - YearsOfService=6, VestingPercent=100
/// </remarks>
public sealed class VestingScheduleDetail
{
    public int Id { get; set; }

    public int VestingScheduleId { get; set; }

    /// <summary>
    /// Number of years of service required for this vesting percentage.
    /// </summary>
    public int YearsOfService { get; set; }

    /// <summary>
    /// Vesting percentage (0-100) for this years of service.
    /// </summary>
    public decimal VestingPercent { get; set; }

    /// <summary>
    /// Navigation property to the parent vesting schedule.
    /// </summary>
    public VestingSchedule VestingSchedule { get; set; } = null!;
}
