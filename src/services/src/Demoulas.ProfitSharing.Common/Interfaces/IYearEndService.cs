namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IYearEndService
{
    Task RunFinalYearEndUpdatesAsync(short profitYear, bool rebuild, CancellationToken ct);

    Task UpdateEnrollmentIdAsync(short profitYear, CancellationToken ct);

    // Gets the last completed year-end update.  For example, if invoked today 11/2025 - the last completed year end is 2024.
    Task<short> GetCompletedYearEndAsync(CancellationToken ct);

    // Gets the Open Profit Year. or Active Profit Year.  The backend determines if we are actively working on a Year End.
    // The algo is to look for a year which is after the "last completed YE" and on for which an active Frozen profit year exists.
    // For example today is 2025, the last completed YE was 2024.  And there is an active frozen row for 2025.  So the open
    // profit year is 2025.
    Task<short> GetOpenProfitYearAsync(CancellationToken ct);

}
