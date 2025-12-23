using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.CheckRun;

/// <summary>
/// Request to start a new profit sharing check run.
/// Initiates the complete workflow: file generation (MICR, DJDE, PositivePay) → FTP transfer → audit logging.
/// </summary>
public sealed record CheckRunStartRequest : IProfitYearRequest
{
    /// <summary>
    /// The profit year for which checks are being issued (e.g., 2024).
    /// Must be a valid year between 2000 and 2100.
    /// </summary>
    public required short ProfitYear { get; init; }

    /// <summary>
    /// The date on which checks are issued (e.g., 2024-12-25).
    /// Used for check metadata and audit trail. Must be a valid date.
    /// </summary>
    public required DateOnly CheckRunDate { get; init; }

    /// <summary>
    /// The check number prefix or starting number for this run (e.g., 7542).
    /// Used in file naming and tracking. Must be a positive integer.
    /// </summary>
    public required int CheckNumber { get; init; }

    /// <summary>
    /// The unique identifier of the user initiating the check run.
    /// Used for audit logging and authorization tracking.
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Example request for documentation and testing.
    /// </summary>
    public static CheckRunStartRequest RequestExample() => new()
    {
        ProfitYear = 2024,
        CheckRunDate = new DateOnly(2024, 12, 25),
        CheckNumber = 7542,
        UserId = Guid.NewGuid()
    };
}
