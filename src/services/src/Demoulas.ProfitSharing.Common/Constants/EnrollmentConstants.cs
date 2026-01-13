namespace Demoulas.ProfitSharing.Common.Constants;

/// <summary>
/// Constants for enrollment status values.
/// These values indicate if an employee is new, returning, or has forfeited and/or left the plan.
/// </summary>
/// <remarks>
/// Previously stored in the ENROLLMENT lookup table, now maintained as constants since
/// the enrollment status is computed dynamically based on profit detail transaction history.
/// See EnrollmentSummarizer for the computation logic (extracted from PAY450.cbl).
/// </remarks>
public static class EnrollmentConstants
{
    /// <summary>
    /// Employee is not enrolled in the profit sharing plan (no contributions or forfeitures).
    /// </summary>
    public const byte NotEnrolled = 0;

    /// <summary>
    /// Employee is enrolled under the old vesting plan (7 years to full vesting) and has contribution records.
    /// Effective for employees whose first contribution was before 2007.
    /// </summary>
    public const byte OldVestingPlanHasContributions = 1;

    /// <summary>
    /// Employee is enrolled under the new vesting plan (6 years to full vesting) and has contribution records.
    /// Effective for employees whose first contribution was in 2007 or later.
    /// </summary>
    public const byte NewVestingPlanHasContributions = 2;

    /// <summary>
    /// Employee was enrolled under the old vesting plan but has forfeiture records (left the plan before fully vested).
    /// </summary>
    public const byte OldVestingPlanHasForfeitureRecords = 3;

    /// <summary>
    /// Employee was enrolled under the new vesting plan but has forfeiture records (left the plan before fully vested).
    /// </summary>
    public const byte NewVestingPlanHasForfeitureRecords = 4;

    /// <summary>
    /// Import status is unknown for employees from legacy systems where historical enrollment data was not tracked.
    /// </summary>
    public const byte Import_Status_Unknown = 9;

    /// <summary>
    /// Gets a human-readable description for an enrollment status code.
    /// </summary>
    /// <param name="enrollmentId">The enrollment status code.</param>
    /// <returns>A descriptive string for the enrollment status.</returns>
    public static string GetDescription(byte enrollmentId) => enrollmentId switch
    {
        NotEnrolled => "Not Enrolled",
        OldVestingPlanHasContributions => "Old vesting plan has Contributions (7 years to full vesting)",
        NewVestingPlanHasContributions => "New vesting plan has Contributions (6 years to full vesting)",
        OldVestingPlanHasForfeitureRecords => "Old vesting plan has Forfeiture records",
        NewVestingPlanHasForfeitureRecords => "New vesting plan has Forfeiture records",
        Import_Status_Unknown => "Previous years enrollment is unknown. (History not previously tracked)",
        _ => $"Unknown enrollment status ({enrollmentId})"
    };

    /// <summary>
    /// Determines if an enrollment status indicates the employee is enrolled (has contributions or forfeitures).
    /// </summary>
    /// <param name="enrollmentId">The enrollment status code.</param>
    /// <returns>True if enrolled (status 1-4), false otherwise.</returns>
    public static bool IsEnrolled(byte enrollmentId) => enrollmentId is 
        OldVestingPlanHasContributions or 
        NewVestingPlanHasContributions or 
        OldVestingPlanHasForfeitureRecords or 
        NewVestingPlanHasForfeitureRecords;

    /// <summary>
    /// Determines if an enrollment status indicates the employee has forfeited.
    /// </summary>
    /// <param name="enrollmentId">The enrollment status code.</param>
    /// <returns>True if forfeited (status 3 or 4), false otherwise.</returns>
    public static bool HasForfeited(byte enrollmentId) => enrollmentId is 
        OldVestingPlanHasForfeitureRecords or 
        NewVestingPlanHasForfeitureRecords;

    /// <summary>
    /// Determines if an enrollment status uses the old vesting plan (7-year schedule).
    /// </summary>
    /// <param name="enrollmentId">The enrollment status code.</param>
    /// <returns>True if uses old vesting plan (status 1 or 3), false otherwise.</returns>
    public static bool UsesOldVestingPlan(byte enrollmentId) => enrollmentId is 
        OldVestingPlanHasContributions or 
        OldVestingPlanHasForfeitureRecords;

    /// <summary>
    /// Determines if an enrollment status uses the new vesting plan (6-year schedule).
    /// </summary>
    /// <param name="enrollmentId">The enrollment status code.</param>
    /// <returns>True if uses new vesting plan (status 2 or 4), false otherwise.</returns>
    public static bool UsesNewVestingPlan(byte enrollmentId) => enrollmentId is 
        NewVestingPlanHasContributions or 
        NewVestingPlanHasForfeitureRecords;
}
