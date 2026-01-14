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
    /// The set of distributions to print checks for.
    /// </summary>
    public required IReadOnlyCollection<long> DistributionIds { get; init; }

    /// <summary>
    /// Which printer/output format to generate.
    /// </summary>
    public required CheckRunPrinterType PrinterType { get; init; }

    /// <summary>
    /// When true, voids prior checks for the run and generates new check numbers.
    /// </summary>
    public bool IsReprint { get; init; }

    /// <summary>
    /// The username of the user initiating the check run.
    /// Used for audit logging and authorization tracking.
    /// </summary>
    public required string UserName { get; init; }

    /// <summary>
    /// Example request for documentation and testing.
    /// </summary>
    public static CheckRunStartRequest RequestExample() => new()
    {
        ProfitYear = 2024,
        CheckRunDate = new DateOnly(2024, 12, 25),
        DistributionIds = new long[] { 10001, 10002, 10003 },
        PrinterType = CheckRunPrinterType.XeroxDjde,
        IsReprint = false,
        UserName = "jdoe"
    };
}
