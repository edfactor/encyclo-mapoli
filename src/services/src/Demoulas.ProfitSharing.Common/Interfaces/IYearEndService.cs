namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IYearEndService
{
    Task RunFinalYearEndUpdates(short profitYear, bool rebuild, CancellationToken ct);

    Task UpdateEnrollmentId(short profitYear, CancellationToken ct);

    // Gets the last completed year-end update.  For example, if invoked today 11/2025 - the last completed year end is 2024.
    Task<short> GetCompletedYearEnd(CancellationToken ct);
}
