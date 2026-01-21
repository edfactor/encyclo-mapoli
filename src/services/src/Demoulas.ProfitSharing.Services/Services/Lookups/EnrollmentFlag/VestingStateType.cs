namespace Demoulas.ProfitSharing.Services.EnrollmentFlag;

/// <summary>
/// Represents the vesting state of an employee based on years of service.
/// </summary>
internal enum VestingStateType
{
    /// <summary>
    /// Employee has zero vesting (0 years of service).
    /// </summary>
    NotVested,

    /// <summary>
    /// Employee is partially vested (1-5 years for new plan, 1-6 years for old plan).
    /// </summary>
    PartiallyVested,

    /// <summary>
    /// Employee is fully vested (6+ years for new plan, 7+ years for old plan).
    /// </summary>
    FullyVested
}
